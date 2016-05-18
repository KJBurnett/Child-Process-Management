# Child Process Management
Observing child processes created from parent processes.

## What is this?
 - I'll be specifically speaking for .NET (in C#!)
 - In C#, you can start a variety of processes by using a built in class "System.Diagnostics.Process". In the following code block, I use Minecraft as an example program that I want to start with the Process class. when I close the MinecraftLauncher.exe window, the code will continue past "proc.WaitForExit();
 
```C#
Process proc = Process.Start("F:\\Games\\Minecraft\\MinecraftLauncher.exe");
  proc.WaitForExit();
  Console.WriteLine("Process has exited");
```

- There is a problem with this code though, even though the process has exited, there is the possibility that the user pressed the "PLAY" button within the launcher, this would close the launcher and open a new child process containing the actual game. This means that if you wished to record the length of the program running, where "proc.WaitForExit()" finished is not the true end of its life cycle.

- Here's a fix called GetChildProcesses. This method specifically will use the ManagementObjectSearcher to return an IEnumerable list of child processes that the parent created.

```C#
public static IEnumerable<Process> GetChildProcesses(this Process process)
{
    Console.WriteLine("Entering GetChildProcesses (IEnumerable)");
    List<Process> children = new List<Process>();
    ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

    foreach (ManagementObject mo in mos.Get())
    {
        children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
        Console.Write("NEW PROCESS: ");
        Console.WriteLine(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
    }

    return children;
}
```

- Finally, we can finish this program by adding to each process returned from "GetChildProcesses" an eventhandler for when the process is exited.

```C#
foreach(Process process in processList)
{
    childCount++; // just a counter to see how many childs are exited.
    process.EnableRaisingEvents = true;
    process.Exited += (exitSender, exit_e) => childProcess_Exited(sender, e, process);
}
```

- This last bit of code should help you keep track of when each and every child process is finished.
- We can finish the code by creating the childProcess_Exited event method stated in the lambda in the foreach method above.

```C#
private void childProcess_Exited(object sender, System.EventArgs e, Process process)
 {
     childsExited++;
     Console.WriteLine("Child Process Exited: " + process.ProcessName);

     if (childsExited == childCount)
         Console.WriteLine("All child processes have exited.");
 }
```
