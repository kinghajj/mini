Mini, an INI library for the .NET framework.

Mini is free software: you can redistribute it and/or modify it under the
terms of the GNU Lesser General Public License as published by the Free
Software Foundation, either version 2.1 of the License, or (at your option)
any later version.

Mini is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
details.

You should have received a copy of the GNU Lesser General Public License
along with Mini. If not, see <http://www.gnu.org/licenses/>.

================================================================================

1. About Mini

Mini is an INI library for the .NET framework. It aims to be small and easy-to-
use while providing functionality lacking in other INI libraries. Specifically,
Mini associates comments with sections or settings if they're adjoined. This is
so that you can put useful comments in the INI, then display those comments in
another program, such as a configuration program; in such a case, the comments
could be used to explain different possible settings to the user.

I started writing Mini because I thought it could be useful to use at my work.
I tried using Nini, another INI library for the .NET framework, but it didn't
have all the features I required, and hasn't been updated for some time.

================================================================================

2. How Mini Works

Mini is pretty simple. When you load an INI document, it's parsed into an
internal structure. You can then change that structure programatically, then
save that structure back to a file.

Parsing an INI document is rather simple. There are only three parts: comments,
section headers, and settings. Settings are associated with the most recently
seen section header, and comments may stand alone or be associated with a
section header or setting. A comment will be associated with a section header
or setting if it appears on the same line or on the lines immediately before
them. Additionally, adjacent comments are assumed to be one, multi-line comment.

Here's a simple example of how this works.

    ; This comment is all by itself--it's not associated with anything.
    
    
    ; This comment is associated with the following section.
    ; This is a continuation of the previous comment.
    [Foo] ; Yet another comment on this section!
    ; This comment is associated with the following setting.
    Bar = Baz

If this gets parsed then re-written unchanged, the file will look like this.

    ; This comment is all by itself--it's not associated with anything.
    
    ; This comment is associated with the following section.
    ; This is a continuation of the previous comment.
    ; Yet another comment on this section!
    [Foo]
    ; This comment is associated with the following setting.
    Bar = Baz

As you can see, whether a comment appeared on the same line or the previous
lines is not stored, so when re-written, all comments are put before whatever
they are associated with. You can also see that extraneous newlines are not
kept.

================================================================================

3. How to Use Mini

Using Mini is very straightforward.

    var ini = new IniDocument("test.ini");
    ini["Section"]["Setting"].Value = "Foo";
    ini.Write();

================================================================================

4. How does it perform?

On my computer, Mini can read and parse an INI document at about 5 MiB/s, and
can write an in-memory INI document back to a file at about 20 MiB/s. The test
program, which opens a file, changes it slightly, and then writes it back to
a second file, runs at about 4 MiB/s. These results were calculated with a real-
world, half-mebibyte INI file. This file contains hundreds of sections, each
with dozens of settings and comments, so I consider it a fair measure of Mini's
parsing speed.

The test program also tests how fast common operations like looking up a section
or setting by name are by repeating the same seven operations 1000 times. This
takes about 100 milliseconds on my machine, which translates to 70,000
operations per second.

================================================================================

5. What does "Mini" mean?

Maybe "Mini: INI .NET Interface"?
