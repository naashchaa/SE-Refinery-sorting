using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        
        MyCommandLine _commandline;
        Dictionary<String, Action> _commands;
        IMyBlockGroup groups;
        IMyRefinery sortRefinery;
        IMyInventory refInv;
        int[] oreIndices;



        public Program()                                                                //Constructor
        {
            
            
            _commandline = new MyCommandLine();
            _commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

            oreIndices = new int[11];

            _commands["sort"] = Sort;
            _commands["reset"] = Reset;
            _commands["stockpile"] = Stockpile;

        }

        public bool checkSortedSpeed(IMyInventory inv)
        {

            for (int i = 0; i < oreIndices.Length; i++)
            {
                oreIndices[i] = -1;
            }



            for (int i = 0; i < 11; i++)        //Speed Sort Order: 0 stone > 1 scrap > 2 iron > 3 gold > 4 silicon > 5 mg > 6 ag > 7 nickel > 8 plat > 9 cobalt > 10 U
            {

                if (inv.GetItemAt(i) == null)
                {
                    break;
                }

                MyInventoryItem testItem = (MyInventoryItem)inv.GetItemAt(i);   //Assigning current position to array of ores to store ore position
                
                if (testItem.Type.SubtypeId == "Stone")
                {
                    oreIndices[0] = i;
                }

                if (testItem.Type.SubtypeId == "Scrap")
                {
                    oreIndices[1] = i;
                }

                if (testItem.Type.SubtypeId == "Iron")
                {
                    oreIndices[2] = i;
                }

                if (testItem.Type.SubtypeId == "Gold")
                {
                    oreIndices[3] = i;
                }

                if (testItem.Type.SubtypeId == "Magnesium")
                {
                    oreIndices[4] = i;
                }

                if (testItem.Type.SubtypeId == "Silicon")
                {
                    oreIndices[5] = i;
                }

                if (testItem.Type.SubtypeId == "Nickel")
                {
                    oreIndices[6] = i;
                }

                if (testItem.Type.SubtypeId == "Silver")
                {
                    oreIndices[7] = i;
                }

                if (testItem.Type.SubtypeId == "Platinum")
                {
                    oreIndices[8] = i;
                }

                if (testItem.Type.SubtypeId == "Cobalt")
                {
                    oreIndices[9] = i;
                }

                if (testItem.Type.SubtypeId == "Uranium")
                {
                    oreIndices[10] = i;
                }

            }

            int currentSmallest = 12;                           //Checking to see if inventory is already sorted according to predefined sorting pattern
            for (int i = oreIndices.Length - 1; i >= 0; i--)
            {
                if (oreIndices[i] != -1)
                {
                    if (oreIndices[i] < currentSmallest)
                        currentSmallest = oreIndices[i];
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Sort()          //The Sort method. The main part of the script
        {
            string sortObject = _commandline.Argument(1);
            string sortContainer = _commandline.Argument(2);
            string sortType = _commandline.Argument(3);

            bool sortToStorage = _commandline.Switch("usestorage");
            bool sortBlockGroups = _commandline.Switch("usegroups");

            

            if (sortBlockGroups && sortObject != null)
            {
                groups = GridTerminalSystem.GetBlockGroupWithName(sortObject);
            }
            else if (!sortBlockGroups && sortObject != null)
            {
                sortRefinery = GridTerminalSystem.GetBlockWithName(sortObject) as IMyRefinery;
                refInv = sortRefinery.InputInventory;
            }
            else
            {
                Echo("sortObject is null.");
                return;
            }

            if (sortContainer == null && sortToStorage)
            {
                Echo("sortContainer is null.");
                return;
            }

            if (sortType == null)
            {
                Echo("sortType is null, defaulting to Speed.");
                sortType = "speed";
            }

            //Actual Sorting Begins Here

            int sortingCase;

            if (refInv.ItemCount < 1)
                sortingCase = 0;
            else if (checkSortedSpeed(refInv))
                sortingCase = 2;
            else
                sortingCase = 1;

            if (sortingCase == 1)
            {
                MyInventoryItem item;
                int[] oresCompact = new int[refInv.ItemCount];
                int k = 0;

                for (int i = 0; i < 11; i++)
                {
                    if (oreIndices[i] != -1)
                    {
                        oresCompact[k] = oreIndices[i];
                        Echo($"Array value is {oresCompact[k]}"); 
                        k++;
                    }
                }

                for (int i = 0; i < refInv.ItemCount; i++)
                {
                    item = (MyInventoryItem)refInv.GetItemAt(oresCompact[i]);

                    refInv.TransferItemFrom(refInv, oresCompact[i], refInv.ItemCount, true, item.Amount);

                    for (int j = 0; j < refInv.ItemCount; j++)
                    {
                        if (oresCompact[j] > oresCompact[i])
                            oresCompact[j]--;
                    }
                    oresCompact[i] = refInv.ItemCount;
                }

            }

            //1st case: not sorted and has items in inventory, push to container > pull in appropriate order
            //2nd case: empty, pull in appropriate order
            //3rd case: sorted, no action needed

            //Speed Sort Order: 0 stone > 1 scrap > 2 iron > 3 gold > 4 silicon > 5 mg > 6 ag > 7 nickel > 8 plat > 9 cobalt > 10 U
            //Value Sort Order: 0 U > 1 plat > 2 gold > 3 ag > 4 cobalt > 5 nickel > 6 silicon > 7 mg > 8 iron > 9 scrap > 10 stone

            //Approach 1: inter-refinery sort, put each item into designated position in inventory

        }

        public void Reset()
        {

        }

        public void Stockpile()
        {

        }

        public void Save()                                                              //Save
        {

        }


        public void Main(string argument, UpdateType updateSource)                      //Main
        {


            if (_commandline.TryParse(argument))  //Check for some input
            {
                Action commandAction;
                String inputCommand = _commandline.Argument(0);

                if (inputCommand == null)
                {
                    Echo("Command is null.");
                    return;
                } 
                else if (_commands.TryGetValue(inputCommand, out commandAction))
                {
                    commandAction();
                }
                else
                {
                    Echo($"Unrecognized command {inputCommand}.");
                }


            }
        }
    }
}
