# Explorer

Finds startup applications, or adds applications on any startup group.

```csharp
public static class Explorer
```

## Methods

### **GetAll()**

Returns all startup applications found, sorted by group.

```csharp
public static Dictionary<StartupGroup, StartupApplicationData[]> GetAll()
```
### **GetAllEnabled()**
Returns all enabled startup applications found, sorted by group.
```csharp
public static Dictionary<StartupGroup, StartupApplicationData[]> GetAllEnabled()
```

### **GetGroup(StartupGroup)**

Returns all startup applications found in a specific group.

```csharp
public static StartupApplicationData[] GetGroup(StartupGroup startupGroup)
```

### **GetGroupEnabled(StartupGroup)**

Returns all enabled startup application found in a specific group.

```csharp
public static List<StartupApplicationData> GetGroupEnabled(StartupGroup startupGroup)
```

### **AddToStartup(String, String, StartupGroup)**

Adds a program to windows startup in the specified startup group. 
 `appName` can be whatever.
 `appPath` requires the executable file as well (ex. C:\Foo.exe).
 If the `startupGroup` chosen is one of the folders it creates a shortcut to it, otherwise it adds a value to the corresponding registry key.

```csharp
public static void AddToStartup(string appName, string appPath, StartupGroup startupGroup)
```

---
# StartupApplicationData

Contains data about a single application found in one of the startup groups. Has a few useful methods to change whether this specific application is enabled to run on startup, or remove it from startup entirely.

```csharp
public class StartupApplicationData
```

## Fields

### **name**

Display name of the shortcut or key value. Unimportant in registry entries.

```csharp
public readonly string name;
```

### **rawPath**

The path of the executable. Includes arguments for registry entries.

```csharp
public readonly string rawPath;
```

### **executablePath**

The path of the executable. Does NOT include arguments.

```csharp
public readonly string executablePath;
```

### **arguments**

Arguments for registry entries.

```csharp
public readonly string arguments;
```

### **groupPath**

The path to the startup group.

```csharp
public readonly string groupPath;
```

### **startupGroup**

Startup group enumerator.

```csharp
public readonly StartupGroup startupGroup;
```

### **requiresAdminPrivilages**

True if this application belongs to a group that requires administrator privilages.

```csharp
public readonly bool requiresAdminPrivilages;
```

## Properties

### **state**

Whether the startup entry corresponding to this object will run on startup.

```csharp
public AppStartupState state { get; private set; }
```

### **isShortcut**

Returns true if this application is in the start menu folders.

```csharp
public bool isShortcut { get; }
```

## Methods

### **SetKeyState(Boolean)**

Changes the registry value determining whether this application will actually run on startup.

```csharp
public void SetKeyState(bool state)
```

### **SetKeyState(AppStartupState)**

Changes the registry value determining whether this application will actually run on startup.

```csharp
public void SetKeyState(AppStartupState state)
```

### **RemoveFromStartup()**

Removes the shortcut or registry entry corresponding to this object.

```csharp
public void RemoveFromStartup()
```

### **SplitPathAndArguments(String, String&, String&)**

Splits the application path and arguments. Every application has its own arguments format so this method is slightly convoluted.

```csharp
public static void SplitPathAndArguments(string fullPath, String& path, String& args)
```

### **SplitPathAndArguments(String&, String&)**

Splits the application path and arguments. Every application has its own arguments format so this method is slightly convoluted.

```csharp
public void SplitPathAndArguments(String& path, String& args)
```

