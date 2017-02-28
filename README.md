# Describer
Simple library for creating string descriptions of objects

The examples use a couple of simple classes to demonstrate the variations

```C#
class TestClass
{
  public string Prop => "first";
  public string Prop2 => "sec";

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
DescriptionBuilder.Describe(new TestClass(),
    o => o.Prop
  )
```
Becomes ```{ Prop: first }```

```C#
DescriptionBuilder.Describe(new TestClass(),
    o => o.MyMethod()
  )
```
Becomes ```{ MyMethod(): Foo }```

```C#
DescriptionBuilder.Describe(new TestClass(),
    o => o.Prop + o.Prop2
  )
```
Becomes ```{ Prop, Prop2: firstsec }```

```C#
DescriptionBuilder.Describe(new TestClass(),
    o => o.Prop,
    o => DescriptionBuilder.Describe(o.ObjProp,
        o2 => o2.MyMethod()
      )
  )
```
Becomes ```{ Prop: first; ObjProp: { MyMethod(): Foo } }```
