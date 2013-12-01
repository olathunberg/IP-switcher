using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Management;

namespace Wireless_Network_Manager.Classes
{
    public class HandleNetworks
    {
        private ProcessStartInfo psi()
        {
            string CmdExePath = Environment.SystemDirectory + "\\cmd.exe";
            string NetshPath = Environment.SystemDirectory + "\\netsh.exe";


            //Check if files exists
            if (!File.Exists(CmdExePath))
            {
                MessageBox.Show("Needed files does not exists. \rErrorcode:1");
                Environment.Exit(1);
            }
            if (!File.Exists(NetshPath))
            {
                MessageBox.Show("Needed file does not exists. \rErrorcode:2");
                Environment.Exit(1);
            }

            //Starting Information for process like its path, use system shell i.e. control process by system etc.
            ProcessStartInfo psi = new ProcessStartInfo(@CmdExePath);
            // its states that system shell will not be used to control the process instead program will handle the process
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            // Do not show command prompt window separately
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            //redirect all standard inout to program
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;

            return psi;

        }

        private Process CMDProc()
        {
            //create the process with process startinfo (PSI) and start it
            Process plinkProcess = new Process();
            plinkProcess.StartInfo = psi();
            plinkProcess.Start();

            return plinkProcess;
        }

        public string CMDPromptHandler(string commandstring)
        {
            //Create the a cmd.exe process
            Process CommandPromptProc = CMDProc();

            //link the streams to standard inout of process
            StreamWriter inputWriter = CommandPromptProc.StandardInput;
            StreamReader outputReader = CommandPromptProc.StandardOutput;
            StreamReader errorReader = CommandPromptProc.StandardError;

            inputWriter.WriteLine(commandstring);

            // flush the input stream before sending exit command to end process for any unwanted characters
            inputWriter.Flush();
            inputWriter.WriteLine("exit\r\n");

            // read till end the stream into string
            CommandPromptProc.Dispose();
            return outputReader.ReadToEnd();
        }

        public string ReadNetworkDetails(string NetworkName)
        {

            //send command to cmd prompt and wait for command to execute
            NetworkName = NetworkName.Insert(0, "\"");
            NetworkName = NetworkName.Insert(NetworkName.Length, "\"");

            return CMDPromptHandler("netsh wlan show profiles name=" + NetworkName + " key=clear");
        }

        public bool DeleteNetwork(string NetworkName)
        {
            //send command to cmd prompt and wait for command to execute
            NetworkName = NetworkName.Insert(0, "\"");
            NetworkName = NetworkName.Insert(NetworkName.Length, "\"");

            //CommandPromptProc.Dispose();
            CMDPromptHandler("netsh wlan delete profile name=" + NetworkName);
            return true;
        }

    }

}
