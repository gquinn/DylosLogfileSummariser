using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

//
//  See the Readme.txt file for info !
//

namespace DylosLogfileSummariser
{
    public partial class Form1 : Form
    {
        const string versionInfo = "Dylos Log File Summariser.  Project Started 14th April 2013.  This version 15th April 2013";
        const string tempSubString = "unsorted_";
        const string helpMessage1 = "Press \"Choose LogFile Directory\" to Locate a Dylos Logs Directory";
        const string helpMessage2 = helpMessage1 + " or press \"Summarise Files\" to process the raw data";

        const string gnuPlotInfo = "Development/Testing graphs with gnuPlot 4.6.0 http://sourceforge.net/projects/gnuplot/files/";
        const string gnuPlotLocation1 = "C:\\Program Files\\gnuplot\\bin\\gnuplot.exe";
        const string gnuPlotLocation2 = "C:\\Program Files (x86)\\gnuplot\\bin\\gnuplot.exe";


        //
        // class constructor
        //
        public Form1()
        {
            InitializeComponent();

            Step.Text = helpMessage1;
            Status.Text = "";
            FileStatus.Text = "";

            WorkingDirectory.Text = "C:\\";

            Version.Text = versionInfo;

            Summarise.Enabled = false;

            GNUPlotLocation.Text = locateDefaultGNUPlotInstallation();

            validateGNUPlotLocation();

        } //  Form1 class constructor

        private string locateDefaultGNUPlotInstallation()
        {
            if (File.Exists(gnuPlotLocation1)) return gnuPlotLocation1;
            if (File.Exists(gnuPlotLocation2)) return gnuPlotLocation2;

            return "";
        }

        //
        // Callback function for the "Choose Logfile Directory" button
        //
        private void DirectoryBrowser_Click(object sender, EventArgs e)
        {
            Status.Text = "";
            FileStatus.Text = "";

            FolderBrowser.SelectedPath = WorkingDirectory.Text;
            FolderBrowser.ShowNewFolderButton = false;

            DialogResult result = FolderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                WorkingDirectory.Text = FolderBrowser.SelectedPath;

                Summarise.Enabled = true;

                Step.Text = helpMessage2;
                Step.Update();
            }

        } // Callback function for DirectoryBrowser_Click

        private bool processDylosLogFile(string dayFilesFolder, string filename)
        {
            bool processed = false;

            Status.Text = "Processing " + filename;
            Status.Update();

            int lineCount = 0;
            string line;

            System.IO.StreamWriter outFile = null;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader( filename );
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 19)
                {
                    lineCount++;

                    string lastOutFilename = "";

                    long small, large;

                    if (line[2] == '/') // we've found a date on this line, indicating the start of some particle count data
                    {
                        Directory.CreateDirectory(dayFilesFolder);

                        string outFilename = dayFilesFolder + "\\" + tempSubString + line.Substring(0, 2) + line.Substring(3, 2) + line.Substring(6, 2) + ".csv";

                        // if the date changes, then we need to write to a different file
                        if (outFilename != lastOutFilename)
                        {
                            if (outFile != null)
                            {
                                if (outFile.BaseStream != null) // writer is already open
                                {
                                    outFile.Close();
                                }
                            }

                            outFile = new System.IO.StreamWriter(outFilename, true); // append to file
                        }

                        int smallIndex = line.IndexOf(',');

                        if (smallIndex > 0)
                        {
                            processed = true;

                            string smallString = line.Substring(smallIndex + 2);
                            int largeIndex = smallString.IndexOf(',');

                            long.TryParse(smallString.Substring(0,largeIndex), out small);
                            long.TryParse(smallString.Substring(largeIndex + 2), out large);

                            outFile.WriteLine(line.Substring(9, 5) + "," + small + "," + large);
                        }
                    }
                }

                Thread.Sleep(0); // yield the CPU if another process is asking for it

            } // while

            if (processed)
            {
                FileStatus.Text = "Processed " + lineCount + " Lines in " + filename;
            }
            else
            {
                FileStatus.Text = "Not processed " + filename;
            }
            FileStatus.Update();

            if (outFile != null)
            {
                if (outFile.BaseStream != null) // writer is open
                {
                    outFile.Close();
                }
            }

            if (file != null)
            {
                if (file.BaseStream != null) // reader is open
                {
                    file.Close();
                }
            }

            return processed;

        } // processLogFile()

        //
        // Dylos files can contain data from several days of monitoring, AND
        // it is possible that several Dylos Log files might contain data from the same date.
        // This function gathers all the data for each monitored day into its own data file.
        //
        private void splitDylosFilesIntoDayFiles(string dayFilesFolder, string dylosFolder)
        {
            int fileCount = 0;

            foreach (string filename in System.IO.Directory.GetFiles(dylosFolder))
            {
                string extension = Path.GetExtension(filename);

                if (extension.ToLower() == ".txt")
                {
                    bool processed = processDylosLogFile(dayFilesFolder, filename);

                    if (processed)
                    {
                        fileCount++;
                    }
                }
            }

            Status.Text = "Done Processing " + fileCount + " Dylos Log files in directory " + dylosFolder;
            Status.Update();

            FileStatus.Text = "";
            FileStatus.Update();

        } // splitDylosFilesIntoDayFiles()

        private void sortAndSummariseDayFiles(string dayFilesFolder)
        {
            int fileCount = 0;

            foreach (string filename in System.IO.Directory.GetFiles(dayFilesFolder))
            {
                fileCount++;

                // Each Day file can have upto one line per minute in a day,
                // that is, 60*24, or 1440 lines.  However, we don't know the actual number until we've read them, 
                // so a List of strings is used here.
                List<String> fileContents = new List<String>();

                // Read the file and display it line by line.
                System.IO.StreamReader file = new System.IO.StreamReader( filename );

                if (file != null)
                {
                    string line;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            fileContents.Add(line);
                        }
                    }

                    file.Close();
                }

                if (fileContents.Count > 0) // if there was any data in the file
                {
                    fileContents.Sort();

                    int lastTempSubStringPosition = filename.LastIndexOf(tempSubString);
                    string sortedFilename = filename.Substring(0, lastTempSubStringPosition) +
                                            "Day_" + filename.Substring(lastTempSubStringPosition + tempSubString.Length);

                    System.IO.StreamWriter outFile = new System.IO.StreamWriter(sortedFilename);

                    if (outFile != null)
                    {
                        outFile.WriteLine("Time,Small Particle Count,Large Particle Count");

                        string xticsString = "set xtics axis (";

                        int lastTicPos = -10;

                        for (int i = 0; i < fileContents.Count; i++)
                        {
                            outFile.WriteLine(fileContents[i]);

                            if (fileContents[i].Length > 5)
                            {
                                // (i - lastTicPos > 5)  prevents tics on adjacent values (Dylos DC1100 sometimes reports two values in the same minute)
                                if ((fileContents[i].Substring(3, 2) == "00") && (i - lastTicPos > 5))
                                {
                                    if (lastTicPos >= 0) // then we have previously written a tic, so we need a comma
                                    {
                                        xticsString = xticsString + ", ";
                                    }

                                    //xticsString = xticsString + " \"" + fileContents[i].Substring(0, 5) + "\", " + (i + 1);
                                    xticsString = xticsString + (i + 0);
                                    lastTicPos = i;
                                }
                            }
                        }

                        xticsString = xticsString + ")";
                        outFile.Close();

                        createGnuPlotConfigFile(xticsString, sortedFilename);
 
                        FileStatus.Text = "Written " + fileContents.Count + " Lines to " + sortedFilename;
                        FileStatus.Update();
                    }

                    // summarise the file into averaged hours

                    string averagedHourFilename = filename.Substring(0, lastTempSubStringPosition) +
                        "Hours_" + filename.Substring(lastTempSubStringPosition + tempSubString.Length);

                    List<string>    hourNumber      = new List<string>();
                    List<double>    smallAverage    = new List<double>();
                    List<double>    largeAverage    = new List<double>();
                    List<int>       numberOfEntries = new List<int>();
                    List<long>      smallMinVal     = new List<long>();
                    List<long>      largeMinVal     = new List<long>();
                    List<long>      smallMaxVal     = new List<long>();
                    List<long>      largeMaxVal     = new List<long>();

                    int     lastHour = -1;
                    int     hour;
                    int     numberOfEntriesThisHour = 0;

                    double  thisHoursSmallTotal = 0.0;
                    double  thisHoursLargeTotal = 0.0;

                    int     comma1Position;
                    int     comma2Position;

                    string  smallValueString;
                    string  largeValueString;

                    string  lastHourString="";

                    long    small;
                    long    large;


                    long    smallMin = 99999999;  // These values are not used
                    long    smallMax = 99999999;  // but we need to initialise 
                    long    largeMin = 0;         // the variables to 
                    long    largeMax = 0;         // keep the compiler happy.


                    for (int i = 0; i < fileContents.Count; i++) // foreach data entry in the file
                    {
                        comma1Position   = fileContents[i].IndexOf(',');
                        comma2Position   = fileContents[i].LastIndexOf(',');
                        smallValueString = fileContents[i].Substring(comma1Position+1,comma2Position-comma1Position-1);
                        largeValueString = fileContents[i].Substring(comma2Position+1);

                        small = long.Parse(smallValueString);
                        large = long.Parse(largeValueString);

                        hour  = int.Parse(fileContents[i].Substring(0, 2));

                        if (i == 0) // initialisation
                        {
                            lastHour = hour;
                            lastHourString = fileContents[i].Substring(0, 5);
                            numberOfEntriesThisHour = 0;

                            smallMin = small;
                            smallMax = small;
                            largeMin = large;
                            largeMax = large;
                        }

                        // if the hour number changes, OR
                        // this is the last entry in the file then
                        // add the previous entry into the list of entries
                        if ((hour != lastHour) || (i == (fileContents.Count - 1)) ) 
                        {
                            hourNumber.Add(lastHourString);
                            smallAverage.Add(thisHoursSmallTotal / (double)(numberOfEntriesThisHour));
                            largeAverage.Add(thisHoursLargeTotal / (double)(numberOfEntriesThisHour));

                            numberOfEntries.Add(numberOfEntriesThisHour);
                            smallMinVal.Add(smallMin);
                            largeMinVal.Add(largeMin);
                            smallMaxVal.Add(smallMax);
                            largeMaxVal.Add(largeMax);


                            lastHour = hour;

                            thisHoursSmallTotal = 0;
                            thisHoursLargeTotal = 0;
                            numberOfEntriesThisHour = 0;

                            smallMin = small;
                            smallMax = small;
                            largeMin = large;
                            largeMax = large;

                            lastHourString = fileContents[i].Substring(0, 5);
                        }

                        // accumulate the total for the hour
                        thisHoursSmallTotal += small;
                        thisHoursLargeTotal += large;
                        numberOfEntriesThisHour++;

                        if (small < smallMin) smallMin = small;
                        if (small > smallMax) smallMax = small;
                        if (large < largeMin) largeMin = large;
                        if (large > largeMax) largeMax = large;

                    } //for (int i = 0; i < fileContents.Count; i++) // foreach data entry in the file

                    // Write the average hour values into a new file
                    createGnuPlotConfigFile("", averagedHourFilename);
                    System.IO.StreamWriter hourFile = new System.IO.StreamWriter(averagedHourFilename);

                    if (hourFile != null)
                    {
                        hourFile.WriteLine("Hour," + 
                                           "Small Particle Count Hourly Average," + 
                                           "Large Particle Count Hourly Average," + 
                                           "Number Of Values This Hour," + 
                                           "Min Small PC Value," + 
                                           "Max Small PC Value," + 
                                           "Min Largearge PC Value," + 
                                           "Max Large PC Value");

                        for (int i = 0; i < hourNumber.Count; i++)
                        {
                            hourFile.WriteLine(hourNumber[i] + "," + 
                                               String.Format("{0:0.00}", smallAverage[i]) + "," + 
                                               String.Format("{0:0.00}", largeAverage[i]) + "," + 
                                               numberOfEntries[i] + "," + 
                                               smallMinVal[i] + "," + 
                                               smallMaxVal[i] + "," + 
                                               largeMinVal[i] + "," +
                                               largeMaxVal[i]);
                        }
                        hourFile.Close();

                        FileStatus.Text = "Written " + hourNumber.Count + " Lines to " + averagedHourFilename;
                        FileStatus.Update();
                    }

                    hourNumber.Clear();
                    smallAverage.Clear();
                    largeAverage.Clear();
                    numberOfEntries.Clear();
                    smallMinVal.Clear();
                    largeMinVal.Clear();
                    smallMaxVal.Clear();
                    largeMaxVal.Clear();

                } // if (fileContents.Count > 0)

                fileContents.Clear(); // finished with the file contents

                Thread.Sleep(0); // Don't hog the CPU

            } // foreach filename
        
            FileStatus.Text = "";
            FileStatus.Update();

        } // sortAndSummariseDayFiles()

        private void createGnuPlotConfigFile(string xticsString, string csvFilename)
        {
            int filenameLen = csvFilename.Length;

            if ((filenameLen > 10) && (csvFilename.IndexOf(tempSubString) != 0)) // don't plot the unsorted files
            {
                string gnuConfFile = csvFilename.Substring(0, csvFilename.Length - 3) + "gnuPlot";
                string pngFilename = (csvFilename.Substring(0, csvFilename.Length - 3) + "png").Replace("\\", "/"); // Windows likes "\", gnuPlot like "/" directory seperators
                string gnuCompatibleCSVFilename = csvFilename.Replace("\\", "/");

                int month = int.Parse(csvFilename.Substring(filenameLen - 10, 2));
                int date = int.Parse(csvFilename.Substring(filenameLen - 8, 2));
                int year = 2000 + int.Parse(csvFilename.Substring(filenameLen - 6, 2));

                DateTime d = new DateTime(year, month, date);

                System.IO.StreamWriter gnuFile = new System.IO.StreamWriter(gnuConfFile);

                if (gnuFile != null)
                {
                    gnuFile.WriteLine("set terminal png");
                    gnuFile.WriteLine("set datafile separator \',\'");
                    gnuFile.WriteLine("set key autotitle columnhead");
                    gnuFile.WriteLine("set xlabel \"Time\"");
                    gnuFile.WriteLine("set ylabel \"Particle Count per cubic foot\"");
                    gnuFile.WriteLine("set title \"" + d.ToString("D") + "\"");

                    gnuFile.WriteLine("set output \"" + pngFilename + "\"");
                    gnuFile.WriteLine("set xtics nomirror rotate by -90");

                    if (xticsString.Length > 0)
                    {
                        gnuFile.WriteLine(xticsString);
                        gnuFile.WriteLine("plot \"" + gnuCompatibleCSVFilename + "\" using 1:2 with lines, \"\" using 1:3 with lines");
                    }
                    else
                    {
                        gnuFile.WriteLine("plot \"" + gnuCompatibleCSVFilename + "\" using 1:2 with lines, \"\" using 1:3 with lines, \"\" using 1:xticlabels(1)");
                    }

                    gnuFile.Close();
                }

            }
        }

        private void createGnuBatchFile(string dayFilesFolder)
        {
            System.IO.StreamWriter batFile = new System.IO.StreamWriter(dayFilesFolder + "\\RunGnuPlot.bat");

            if (batFile == null)
            {
                return;
            }

            batFile.WriteLine("REM @ECHO OFF");
            batFile.WriteLine("REM");
            batFile.WriteLine("REM Automatically generated Batch file " + DateTime.Now.ToString("D"));
            batFile.WriteLine("REM Create graphs for each csv file in the directory");
            batFile.WriteLine("REM");
            batFile.WriteLine("");

            foreach (string filename in System.IO.Directory.GetFiles(dayFilesFolder,"*.gnuPlot"))
            {
                batFile.WriteLine("\"" + GNUPlotLocation.Text + "\" \"" + filename + "\"");
            }

            batFile.WriteLine("");
            batFile.WriteLine("pause");
            batFile.WriteLine("");
            batFile.Close();
 
        }

        //
        // Callback function for the "Summarise" button
        //
        private void Summarise_Click(object sender, EventArgs e)
        {
            //
            //  Backup any existing summary data.
            //
            string dylosFolder      = WorkingDirectory.Text;
            string dayFilesBackup   = dylosFolder + "\\OldDayFiles";
            string dayFilesFolder   = dylosFolder + "\\DayFiles";

            // recursively delete the backup directory and contents
            if (Directory.Exists(dayFilesBackup))
            {
                Directory.Delete(dayFilesBackup, true); 
            }

            if (Directory.Exists(dayFilesFolder)) // rename the existing day folder as a backup
            {
                try
                {
                    Directory.Move(dayFilesFolder, dayFilesBackup);
                }
                catch (Exception)
                {
                    DialogResult result1 = MessageBox.Show("Unable to create a backup of the current outputfolder " + dayFilesFolder,
                                                           "Check for open files and/or directories", MessageBoxButtons.OK);
                    return;
                }
            }

            Directory.CreateDirectory(dayFilesFolder);

            //
            // Disable GUI buttons while we process the data
            //
            DirectoryBrowser.Enabled = false;
            Summarise.Enabled = false;
            UseGNUPlot.Enabled = false;
            LocateGNUPlot.Enabled = false;

            // Seperate monitoring data into seperate files for each day
            Step.Text = "Step 1.  Process Raw data files.";
            Step.Update();

            splitDylosFilesIntoDayFiles(dayFilesFolder, dylosFolder);

            // Sort each day's file into time order
            // and produce an hourly average file for each day
            Step.Text = "Step 2.  Create Hourly Average Files.";
            Step.Update();

            sortAndSummariseDayFiles(dayFilesFolder);


            // Produce config files for gnuPlot
            Step.Text = "Step 3.  Produce gnuPlot config files and create graphs.";
            Step.Update();

            if (UseGNUPlot.Checked && validateGNUPlotLocation())
            {
                createGnuBatchFile(dayFilesFolder);
            }


            // Automate the graph production for each day and hour summary file
            // Still todo

            Step.Text = "Done.";
            Step.Update();
            
            //
            //  We've finished processing, so the GUI can be re-enabled.
            //
            DirectoryBrowser.Enabled = true;
            Summarise.Enabled        = true;
            UseGNUPlot.Enabled       = true;
            LocateGNUPlot.Enabled    = true;

        } // callback function for Summarise_Click

        //
        // Set the colour of the GNUPlot file location textbox according
        // whether or not the specified file exists.
        // Returns true if an exe file has been identified
        //
        private bool validateGNUPlotLocation()
        {
            if ( File.Exists(GNUPlotLocation.Text) && (Path.GetExtension(GNUPlotLocation.Text).ToLower() == ".exe") )
            {
                GNUPlotLocation.BackColor = Color.LightGreen;

                return true;
            }
            else // if we get here, the specified exe file does not exist.
            if (UseGNUPlot.Checked)
            {
                    GNUPlotLocation.BackColor = Color.Tomato;
            }
            else
            {
                    GNUPlotLocation.BackColor = Color.LightGray;
            }

            GNUPlotLocation.Update();

            return false;

        } // validateGNUPlotLocation()

        private void UseGNUPlot_CheckedChanged(object sender, EventArgs e)
        {
            validateGNUPlotLocation();

        } // Callback function for UseGNUPlot_CheckedChanged

        private void GNUPlotLocation_TextChanged(object sender, EventArgs e)
        {
            validateGNUPlotLocation();

        } // Callback function for GNUPlotLocation_TextChanged

        private void LocateGNUPlot_Click(object sender, EventArgs e)
        {
            if (File.Exists(GNUPlotLocation.Text))
            {
                GNUPlotBrowser.InitialDirectory = Path.GetDirectoryName(GNUPlotLocation.Text);
                GNUPlotBrowser.FileName = GNUPlotLocation.Text;
            }
            else
            {
                GNUPlotBrowser.InitialDirectory = "C:\\";
                GNUPlotBrowser.FileName = "";
            }

            GNUPlotBrowser.CheckPathExists = true;
            GNUPlotBrowser.DefaultExt = "exe";
            GNUPlotBrowser.Multiselect = false;

            DialogResult result = GNUPlotBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                GNUPlotLocation.Text = GNUPlotBrowser.FileName;
                validateGNUPlotLocation();
            }
        } 

     } // public partial class Form1 : Form

} // namespace DylosLogfileSummariser
