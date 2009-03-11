/* Copyright (C) 2009 Samuel Fredrickson <kinghajj@gmail.com>
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
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            var startMem = GC.GetTotalMemory(false);
            watch.Start();

            var ini = new IniDocument("test.ini");
            ini["User"]["Name"].Value = "md5sum";
            ini["User"]["PasswordHash"].Value = "e65b0dce58cbecf21e7c9ff7318f3b57";
            ini["User"]["RemoveThis"].Value = "This shouldn'd stay.";
            ini["User"].Remove("RemoveThis");
            ini["RemoveThisToo"].Comment = "This better not stay!";
            ini.Remove("RemoveThisToo");
            ini["User"].Comment = "These are the basic user settings.\nDon't modify them unless you know what you're doing.";
            ini.Write("test2.ini");

            watch.Stop();
            var endMem = GC.GetTotalMemory(false);
            Console.WriteLine("{0} Ticks", watch.ElapsedTicks);
            Console.WriteLine("{0} Milliseconds", watch.ElapsedMilliseconds);
            Console.WriteLine("Mem Start: {0}", startMem);
            Console.WriteLine("    End:   {0}", endMem);
        }
    }
}
