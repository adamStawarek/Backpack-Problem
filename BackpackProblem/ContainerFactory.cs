﻿using System;
using System.IO;
using System.Linq;

namespace BackpackProblem
{
    public static class ContainerFactory
    {
        public static Container ReadFromFile(string path, char separator = ' ')
        {
            using (var reader = new StreamReader(path))
            {
                Container container = null;

                int currentLine = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine()?.TrimStart();
                    string[] values = line?.Split(separator);

                    currentLine++;

                    if (currentLine == 1)
                    {
                        int width = Convert.ToInt32(values?[0]);
                        int height = Convert.ToInt32(values?[1]);
                        container = new Container(width, height);
                    }
                    else if (currentLine == 2)
                    {
                        int itemsCount = Convert.ToInt32(values?[0]);
                    }
                    else
                    {
                        int width = Convert.ToInt32(values?[0]);
                        int height = Convert.ToInt32(values?[1]);
                        int value = Convert.ToInt32(values?[2]);
                        var id = container.AllItems.Count + 1;
                        container?.AddItem(new Item(width, height, value, id));
                    }
                }
                return container;
            }
        }
    }
}
