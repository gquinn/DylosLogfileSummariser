﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace DylosLogfileSummariser
{
    public partial class Form1 : Form
    {
        const string tempSubString = "unsorted_";

        public Form1()
        {
            InitializeComponent();

            Step.Text = "Press \"Choose LogFile Directory\" to Locate a Dylos Logs Directory";
            Status.Text = "";
            FileStatus.Text = "";

            WorkingDirectory.Text = "C:\\";

            Summarise.Enabled = false;

        } // public Form1()

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

                Step.Text = "Press \"Choose LogFile Directory\" to Locate a Dylos Logs Directory or \"Summarise Files\" to process the raw data";
                Step.Update();
            }

        } // private void DirectoryBrowser_Click(object sender, EventArgs e)

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

                    //FileStatus.Text = "Line " + lineCount;
                    //FileStatus.Update();

                    string lastOutFilename = "";

                    long small, large;

                    if (line[2] == '/') // we've found a date on this line, indicating the start of some particle count data
                    {
                        //string dayFilesFolder = folderName + "/Dayfiles";
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

                            long.TryParse(smallString.Substring(0,largeIndex-1), out small);
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

        } // private void processLogFile(string filename)

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
        }

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

                if (fileContents.Count > 0)
                {
                    fileContents.Sort();

                    //string sortedFilename = "Day_" + filename.Substring( tempSubString.Length );

                    int lastTempSubStringPosition = filename.LastIndexOf(tempSubString);
                    string sortedFilename = filename.Substring(0, lastTempSubStringPosition) +
                                            "Day_" + filename.Substring(lastTempSubStringPosition + tempSubString.Length);

                    System.IO.StreamWriter outFile = new System.IO.StreamWriter(sortedFilename);

                    if (outFile != null)
                    {
                        for (int i=0; i<fileContents.Count; i++)
                        {
                            outFile.WriteLine(fileContents[i]);
                        }
                        outFile.Close();

                        FileStatus.Text = "Written " + fileContents.Count + " Lines to " + sortedFilename;
                        FileStatus.Update();
                    }

                    // now summarise the file into averaged hours
                    // still todo

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
                    //int     thisHour;
                    int     thisLinesHour;
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


                    long    smallMin=0;
                    long    smallMax=0;
                    long    largeMin=0;
                    long    largeMax=0;


                    for (int i = 0; i < fileContents.Count; i++) // foreach data entry in the file
                    {
                        comma1Position   = fileContents[i].IndexOf(',');
                        comma2Position   = fileContents[i].LastIndexOf(',');
                        smallValueString = fileContents[i].Substring(comma1Position+1,comma2Position-comma1Position-1);
                        largeValueString = fileContents[i].Substring(comma2Position+1);

                        small            = long.Parse(smallValueString);
                        large            = long.Parse(largeValueString);

                        thisLinesHour    = int.Parse(fileContents[i].Substring(0, 2));

                        if (i == 0) // initialisation
                        {
                            lastHour = thisLinesHour;
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
                        if ((thisLinesHour != lastHour) || (i == (fileContents.Count - 1)) ) 
                        {
                            hourNumber.Add(lastHourString);
                            smallAverage.Add(thisHoursSmallTotal / (double)(numberOfEntriesThisHour));
                            largeAverage.Add(thisHoursLargeTotal / (double)(numberOfEntriesThisHour));

                            numberOfEntries.Add(numberOfEntriesThisHour);
                            smallMinVal.Add(smallMin);
                            largeMinVal.Add(largeMin);
                            smallMaxVal.Add(smallMax);
                            largeMaxVal.Add(largeMax);


                            lastHour = thisLinesHour;

                            thisHoursSmallTotal = small;
                            thisHoursLargeTotal = large;
                            numberOfEntriesThisHour = 1;

                            smallMin = small;
                            smallMax = small;
                            largeMin = large;
                            largeMax = large;

                            lastHourString = fileContents[i].Substring(0, 5);
                        }

                        // add this entry into the total for this hour
                        if (thisLinesHour == lastHour)
                        {
                            thisHoursSmallTotal += small;
                            thisHoursLargeTotal += large;
                            numberOfEntriesThisHour++;

                            if (small < smallMin) smallMin = small;
                            if (small > smallMax) smallMax = small;
                            if (large < largeMin) largeMin = large;
                            if (large > largeMax) largeMax = large;
                        }

                    } //for (int i = 0; i < fileContents.Count; i++) // foreach data entry in the file

                    // Write the average hour values into a new file
                    System.IO.StreamWriter hourFile = new System.IO.StreamWriter(averagedHourFilename);

                    if (hourFile != null)
                    {
                        hourFile.WriteLine("Hour Number" + "," +
                                           "Small Particle Count Average" + "," +
                                           "Large Particle Count Average" + "," +
                                           "Number Of Values This Hour" + "," +
                                           "Min Small PC Value" + "," +
                                           "Max Small PC Value" + "," +
                                           "Min Largearge PC Value" + "," +
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

            } // foreach filename
        
            FileStatus.Text = "";
            FileStatus.Update();
        }

        private void Summarise_Click(object sender, EventArgs e)
        {
            DirectoryBrowser.Enabled    = false;
            Summarise.Enabled           = false;

            string dylosFolder      = WorkingDirectory.Text;
            string dayFilesBackup   = dylosFolder + "\\OldDayFiles";
            string dayFilesFolder   = dylosFolder + "\\DayFiles";

            // recursively delete the backup directory and contents
            if (Directory.Exists(dayFilesBackup))
            {
                Directory.Delete(dayFilesBackup, true); 
            }

            // rename the existing day folder as a backup
            if (Directory.Exists(dayFilesFolder))
            {
                Directory.Move(dayFilesFolder, dayFilesBackup);
            }

            Directory.CreateDirectory(dayFilesFolder);


            // Seperate each day's monitoring data into a file for each day
            Step.Text = "Step 1.  Process Raw data files.";
            Step.Update();

            splitDylosFilesIntoDayFiles(dayFilesFolder, dylosFolder);

            // Sort each day's file into time order
            // and produce an hourly average file for each day
            Step.Text = "Step 2.  Create Hourly Average Files.";
            Step.Update();

            sortAndSummariseDayFiles(dayFilesFolder);

            // Automate the graph production for each day and hour summary file
            // Still todo

            Step.Text = "Done.";
            Step.Update();
            
            DirectoryBrowser.Enabled = true;
            Summarise.Enabled           = true;

        }

     } // public partial class Form1 : Form
} // namespace DylosLogfileSummariser
