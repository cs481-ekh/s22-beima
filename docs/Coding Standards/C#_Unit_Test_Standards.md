# BEIMA C# Unit Test Standards

The following are Unit Test standards any developer working on BEIMA should follow. 

These standards focus on C# Unit Testing.

## Naming Convention

Unit test names should follow the format of `InitialState_Action_ExpectedResult`. 

```
[Test]
public async Task IdNotInDatabase_GetDevice_ReturnsNotFound()
```

## Arranging Tests

Each test case should follow the Arrange, Act, Assert (also known as Given, When, Then) pattern. 