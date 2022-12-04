## Description

MonoDI - Designed to replace your singleton solution. Target design platform is Mobile.

## Requirements
- .Net 4x
- Tested on Unity 2020 (Expected to work also on 2019)
- No Dependencies 

## Library includes:
- Dependency Injection Framework
- Tools for reducing your code base (no code gen)
- SignalBus
- Example of systems (your singleton)

## How it works 
- There is an example in MonoDI/Sample
- Each scene should contain a GameObject with MonoDI component attached. Usually the name of this gameObject is 'System'
- Every attached component to the system will be injected. 
- InjectedMono.cs call MonoDI.cs in order to get all references from System. 
- This means you need to derive from InjectedMono. 
- Do NOT use Awake and OnDestroy. There is a separate API for synchronization and destruction.
## Example of use
```c#
public class PlayerController : InjectedMono
{
    [Get] private Rigidbody _rig; // GetComponent
    [Get] private Renderer[] _renderers; // GetComponents (array)
    [In] private InputSystem _inputSystem; // Injects from System
    
    void Update()
    {
        if (_inputSystem.InputData.IsUp)
        {
            _rig.position += Vector3.up * Time.deltaTime;
        }
        
        if (_inputSystem.InputData.IsDown)
        {
            _rig.position += Vector3.down * Time.deltaTime;
        }
        
        if (_inputSystem.InputData.IsLeft)
        {
            _rig.position += Vector3.left * Time.deltaTime;
        }
        
        if (_inputSystem.InputData.IsRight)
        {
            _rig.position += Vector3.right * Time.deltaTime;
        }
    }

    // called by signalBus, check ColorSystem
    // it auto subscribes (Awake) and unsubscribes (OnDestroy)
    [Sub]
    private void OnColorChange(ChangeColorSignal changeColor)
    {
        foreach (var r in _renderers)
        {
            r.sharedMaterial.color = changeColor.ChangeColor;
        }
    }
}

```

### List of attributes

- [In] - Injection from System prefab
- [Get] - GetComponent or GetComponents (array). At the moment no support for List
- [GetChild] - GetComponentInChildren or GetComponentsInChildren (array). At the moment no support for List
- [Find] - FindObjectOfType or FindObjectsOfType (Array).  At the moment no support for List
- [Sub] - Used by SignalBus in order to subscribe, the key is the method parameter. In example above is ChangeColorSignal. 

### How to use SignalBus

Fire:
```C#
public class ColorSystem : InjectedMono
{
    [In] private SignalBus _signal;
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            _signal.Fire(new ChangeColorSignal
            {
                ChangeColor = Color.Lerp(Color.red, Color.green, Random.Range(0f, 1f))
            });
        }
    }
}

public struct ChangeColorSignal : ISignal
{
    public Color ChangeColor;
}
```
Subscribe (Dont forget to Inherit from InjectedMono):
```C#

[Sub]
private void OnColorChange(ChangeColorSignal changeColor)
{
    foreach (var r in _renderers)
    {
        r.sharedMaterial.color = changeColor.ChangeColor;
    }
}
```

## More examples

MonoDI is design for mobile platforms, there are couple of build-in systems(Ads System, Analytics, etc...). 
This systems show the intent of use of this package. 
This systems can be found in MonoDI/Scripts/Systems



