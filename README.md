# UnityAddon3

## Todo
- if a component is disable, resolve it will return null. Should throw exception if the dependency is required.
- preintantiate should be disable for unit test. if preintantiate is removed, all related testcase will passed
- Test on asynclocalfactory

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
