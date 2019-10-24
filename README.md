# UnityAddon3

## Todo
- if a component is disable, resolve it will return null. Should throw exception if the dependency is required.
  - should do strategy validation check before preCreation
- Test on asynclocalfactory
- add instantiate method on containerRegistry
- add interceptors execution order

## Test cases naming convention
TargetUnit

if this test case fails, should go to check which cs file / folder.

WhatIsThisTest

the description of this test is about.

ExpectedResult

the expected result of this test case.

```c#
[Fact]
public void TargetUnit_WhatIsThisTest_ExpectedResult {
    // ...
}
```
