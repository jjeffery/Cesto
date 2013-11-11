Cesto
=====

A collection of useful .NET utilities. These are classes that I have used in a
number of projects.

What's In Cesto
---------------

### Diagnostics

-   `Verify`: Static methods for verifying arguments are not null.

### Disposables

-   `DisposableAction`: Easily create objects that implement `IDisposable` for
    use in C\\# `using` statements.

-   `DisposableExtensions`: Extension methods for `IDisposable` objects. Allows
    an `IDisposable` to be disposed when other objects are disposed.

### Windows Forms

-   `DisplaySettings`: Easily persist UI values across process instances.For
    example, easily persist the position and size of a Windows Form, the widths
    of DataGridView columns, and many other values that you want to persist
    across program instances.

-   `ApplicationInfo`: A bit similar to the Windows Form `Application` class.
    Contains company information, product name, version and the like. Unlike the
    Windows Form `Application` class, this class allows the program to change
    the default values.

-   `RegistryUtils`: Some static classes for accessing the Windows Registry.
    Mainly used to help implement `DisplaySettings`.

-   `WaitCursor`: A simple class to manage display of the wait cursor.

-   `VirtualDataGridViewAdapter`: This class definitely needs a better name. It
    provides a simple way to display large amounts of data in a `DataGridView`.
    It handles data binding, sorting of columns, persistance of column widths,
    and check boxes. I tend to use this class instead of the standard .NET
    databinding, as this class provides superior performance and good sort
    support with very little code.

More Information
----------------

### Cesto and Quokka

For those of you who know me, the intent here is to copy out the most useful
things I use in my Quokka library and make it available via the public nuget
repository.

### Why Cesto?

Not sure where the name "Cesto" comes from. I just wanted a short name that is
not being used in nuget. I think it might mean "Often" in Bosnian.
