/* Copyright (C) 2014 Samuel Fredrickson <samfredrickson@gmail.com>
 * 
 * This file is part of Mini, an INI library for the .NET framework.
 *
 * Mini is free software: you can redistribute it and/or modify it under the
 * terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * Mini is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Mini. If not, see <http://www.gnu.org/licenses/>.
 */

/* Program.cs - A simple test program for Mini.
 */

using System;
using System.Diagnostics;

namespace Mini.Test
{
    [IniValueContainer]
    class Model
    {
        [IniValue("Test", "Foo", "woot")]
        public string Foo { get; set; }

        [IniValue("Test", "Bar", "poot")]
        public string Bar { get; set; }

        [IniValue("Test", "LastRun")]
        public DateTime LastRun { get; set; }
    }

    [IniValueContainer]
    class Model2
    {
        public Model Model { get; set; }
    }

    class Program
    {
        static void Main()
        {
            const int MAX_ITERS = 10000;
            var watch = new Stopwatch();

            // Time how long it takes to process the INI, and estimate how much
            // memory the data structure takes.
            var startMem = GC.GetTotalMemory(false);
            watch.Start();
            var ini = new IniDocument("test.ini");
            watch.Stop();
            var endMem = GC.GetTotalMemory(false);
            Console.WriteLine("Approx. Data Structure Mem: {0}", endMem - startMem);
            Console.WriteLine("{0} ms to process INI.", watch.ElapsedMilliseconds);
            watch.Reset();

            // Time how long it takes to make many simple changes.
            watch.Start();
            for(var i = 0; i < MAX_ITERS; ++i)
            {
                ini["User"]["Name"].Value = "md5sum";
                ini["User"]["Password Hash"].Value = "e65b0dce58cbecf21e7c9ff7318f3b57";
                ini["User"]["Remove This"].Value = "This shouldn'd stay.";
                ini["User"].Remove("Remove This");
                ini["Remove This Too"].Comment = "This better not stay!";
                ini.Remove("Remove This Too");
                ini["User"].Comment = "These are the basic user settings.\nDon't modify them unless you know what you're doing.";
            }
            watch.Stop();
            Console.WriteLine("{0} ms to make changes.", watch.ElapsedMilliseconds);
            watch.Reset();

            // Time how long it takes to deserialize.
            var serializer = IniSerializer<Model2>.New();
            watch.Start();
            var model = serializer.Deserialize(ini);
            for(var i = 0; i < MAX_ITERS - 1; ++i)
                model = serializer.Deserialize(ini);
            model.Model.LastRun = DateTime.Now;
            watch.Stop();
            Console.WriteLine("{0} ms to deserialize.", watch.ElapsedMilliseconds);
            watch.Reset();

            // Time how long it takes to serialize.
            watch.Start();
            for(var i = 0; i < MAX_ITERS; ++i)
                serializer.Serialize(model, ini);
            watch.Stop();
            Console.WriteLine("{0} ms to serialize.", watch.ElapsedMilliseconds);
            watch.Reset();

            // Time how long it takes to write the changes back to another file.
            watch.Start();
            ini.Write("test2.ini");
            watch.Stop();
            Console.WriteLine("{0} ms write INI document to file.", watch.ElapsedMilliseconds);
        }
    }
}
