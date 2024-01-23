# Dvchevskii.Unit

> Simple Unit type

## What is it?

This package provides a type which can only have a single instance.
While it is basically a singleton, the correct usage for it is to
represent the *absense* of a value (i.e. when you need to return **something**, which should never be used)

Basically this is a `void` which can be used as a type argument
