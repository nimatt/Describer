# Describer
Simple library for creating string descriptions of objects

The examples use a couple of simple classes to demonstrate the variations

```C#
class TestClass
{
  public string Prop => "first";
  public string Prop2 => "sec";
  public int NumProp => 5;

  public SecondTestClass ObjProp = new SecondTestClass();

  public string MyMethod()
  {
    return "Foo";
  }
}

class SecondTestClass
{
  public int SecondProp = "second";
}
```

## Examples
```C#
Morpher.Describe(new TestClass(),
    o => o.Prop
  )
```
Becomes ```{"Prop":"first"}```

```C#
Morpher.Describe(new TestClass(),
    o => o.NumProp
  )
```
Becomes ```{"NumProp":5}```

```C#
Morpher.Describe(new TestClass(),
    o => o.MyMethod()
  )
```
Becomes ```{"MyMethod()":"Foo"}```

```C#
Morpher.Describe(new TestClass(),
    o => o.Prop + o.Prop2
  )
```
Becomes ```{"Prop, Prop2":"firstsec"}```

```C#
Morpher.Describe(new TestClass(),
    o => o.Prop,
    o => Morpher.Morph(o.ObjProp,
        o2 => o2.MyMethod()
      )
  )
```
Becomes ```{"Prop":"first","ObjProp":{"MyMethod()":"Foo"}}```
