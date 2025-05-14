# Dynamic Grid for Avalonia

> Disclaimer: This project is not associated in any way with AvaloniaUI, this is just a custom control made by me.

---

This is a custom grid control I created for another personal project and converted into a standalone git repo. 
It is meant to be used with an array or collection of equally sized items (although it will handle differently sized items),
inside of a ScrollViewer.

![functionality showcase](https://github.com/TomasBusch/AvaloniaDynamicGrid/blob/master/images/showcase.gif?raw=true)

## Features
- [x] Dynamic ordering of items.
- [x] Row and Column gap size. 
- [x] Infinite dynamic rows based on content.
- [ ] Per Column/Row dynamic size.
- [ ] Different items spacing methods.

## Installation

### Option 1: Copy the control into your project

1. Download the file DynamicGrid.cs from DynamicGrid.Avalonia
2. Place the file into your AvaloniaUI project solution
3. Change the namespace inside the file to the files path like this:
```c#
//Replace this
namespace DynamicGrid.Avalonia

//With this
namespace YourProject.ControlsDir
```
4. Reference inside your .axaml files like any other custom control this:
```xaml
Remember to change the namespace to your application's specific one
xmlns:dg="clr-namespace:DynamicGrid.Avalonia;assembly=DynamicGrid.Avalonia"
```

### Option 2: Add it to your project as a class library
1. Git clone or download the project into your solution
2. In Visual Studio, add the DynamicGrid.Avalonia class library project as a dependency
3. Reference inside your .axaml files like any other custom control this:
```xaml
xmlns:dg="clr-namespace:DynamicGrid.Avalonia;assembly=DynamicGrid.Avalonia"
```

### Option 3: NuGet Support
Currently I do not plan to publish this project to NuGet, at least not in it's current state.

## Usage

Wrap your DynamicGrid in a ScrollViewer or similar and then add items inside of it.

Like this:
```xaml
<ScrollViewer>
	<dg:DynamicGrid ColumnGap="5" RowGap="5" Margin="10">
		<Panel Width="100" Height="100" Background="red">
			<TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" FontSize="25">1</TextBlock>
		</Panel>
	</dg:DynamicGrid>
</ScrollViewer>
```
* `ColumnGap` sets the minimum size of the gap between columns.
* `RowGap` sets the minimum size of the gap between rows.
* The control has other properties, but currently their functionality is not implemented.

For a full example see the DynamicGrid.Sample project in this repo.

## Known Bugs
* ~~When not allowed to grow infinitely in height (like inside of a ScrollViewer)
the items will begin to overlap each other vertically if there are too many of them.~~(Fixed)
