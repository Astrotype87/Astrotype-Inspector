# Astrotype Inspector

Lightweight and non-destructive inspector attributes, with optional advanced inspector mode.
Solving major problems and annoyances from existing inspector attribute libraries.

## The Problem

After taking enough time using some inspector attribute libraries in my projects and taking a deep look in its source code, certain observations, issues, and smells start to arise. Some libraries' imperfections keeps me from using them.

Safe Implementation
- Most attributes are implemented with PropertyDrawers and DecoratorDrawers which is safe and can work along with other libraries without any issues.

Overriden MonoBehaviour custom editor
- The default MonoBehaviour custom editor are overriden by some libraries, allowing them to implement other attributes like grouping or foldouts.
	- Becuase of this, using multiple inspector attribute libraries in the same project can cause issues.
	- Attributes based on overriden MonoBehaviour custom editor could not work when used with other inspector libraries for the same reason.
- Some libraries implement attributes based on custom editor, but can actually be done with just PropertyDrawer alone.
- If you create your own custom editor, attributes that depends on custom editor will not work.
	- Certain attributes like grouping or tabs not working in your custom editor is acceptable.

Manual Drawing
- Some libraries manually redraw child elements of a class or struct type fields from the ground up, which unfortuntely discards the inspector attribues of child members inside a class/struct foldout.

Visual & Interaction Inconsistencies
- A few libraries have small imperfections in visual details and interactions, or look too far from the native Unity editor look and feel.
- Some UI interactions have small bugs.

Syntax
- Attributes from some libraries, especially with parameters, have unusually long attribute names, inconsistent attribute names, long enum names and values
- Some attributes force you to use explicit optional parameters that gets really long
- Some libraries have good and bad categorization of attributes. Some attributes under category doesn't even feel they have similar purpose.
- Some libraries have names with numbers like Min2 and Range2 which doesn't look good and make me not want to use them

Architecture Complexity
- Some libraries have an entire code architecture of helper classes for drawing any kinds of drawers for each attributes, which makes this library feel heavier as a dependency and less portable.
- Some libraries have base PropertyDrawer which other attribute property drawers inherit from.
- If you want to use any of the attributes for your own Unity C# libraries, you must include the whole package or make them as a dependency, otherwise you have to create your custom editors and attributes yourself.

## The Proposed Solution

Lightweight and Portability
- Each attributes are categorized by folder, where the folder only contains attributes and helper classes in it's scope
	- Each attribute requires certain helper classes and enums, but they are few and small
- Each attribute is a C# script with corresponding PropertyDrawer class inside the same script. One file for one attribute and one property drawer.
	- There are abstract attributes (like ConditionalIf) that are inherited by multiple derived attributes (ShowIf, HideIf, EnableIf, DisableIf), but they all use the same property drawer, and belong to a single file.
- Doesn't use too many special helper classes.
	- Attribute drawers are designed to use helper classes as less as possible.
	- Never uses helper classes to make a core module for drawing properties.
	- PropertyDrawer and DecoratorDrawer implementations and validation are as dumb and isolated as possible.
		- Some code duplication between attribute drawers are by design.
	- Helper/core classes are only used when code duplication are impossible between multiple scripts or the code is too large.
		- Duplicate enums between multiple scripts
		- Functionality that are shared by too many attributes
- No dedicated derived custom PropertyDrawer class.
- No unecessary deep hierarchies.
- Dependencies are lighter and more portable.
- You can "embed" attributes you only need for your Unity C# libraries with instructions:
	- You only need to copy the Core folder, which contains helper classes and enums
	- You MUST change the namespace for your attributes, helper classes, and enums you copied/embedded (preferrably into your library's namespace) to avoid class name conflicts.

Non-Destructive
- Only uses PropertyDrawer and DecoratorDrawer to apply inspector attributes, visual styling and UI interactions.
- You are not forced to override default MonoBehaviour and ScriptableObject editor.
	- Other libraries overrides default custom editor and you don't have an option to disable it. You have to edit the code and comment it out.
	- Option to enable AstrotypeEditor in settings to access advanced features that cannot work with PropertyDrawers alone. This overrides the default custom editor for MonoBehaviour and ScriptableObject.
		- Groups, foldouts, and order attributes
		- Show non-serialized fields, properties and methods in inspector
		- Apply attributes to non-serialized fields, properties, events, methods, parameters, and class/struct declarations
- Never redraws the child elements of a class or struct.
	- Other libraries have poor implementation around class and struct type fields where each child elements are redrawn.
- Respects your custom editors for your MonoBehaviour scripts.
	- Your attributes will not disappear in your custom editors because they are strictly PropertyDrawer.
- Stacks with your own PropertyDrawer if you wish.
	- Simply use these to retains the custom property drawers from your attributes:
		- IMGUI: `EditorGUILayout.PropertyField(property)` or `EditorGUI.PropertyField(position, property, label, true);`
		- UIToolkit: `new PropertyField(property)`

Visual & Interaction Consisteny
- Visual styles of each attribute is aimed to feel like Unity editor.
- UI interactions like typing, label dragging, and updating serialized properties are designed to mimic Unity editor behavior.

Concise Syntax
- Attribute names are short but meaningful at first glance
- Attribute names are consistent, categorized, and easy to know their purpose
- Attributes are as easy to use and organize as possible
- Attribute options are plenty and complete, but conveniently placed
- Enums are more compact and concise.
