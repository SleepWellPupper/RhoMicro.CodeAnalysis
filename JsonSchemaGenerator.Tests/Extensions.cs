namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
using System.Text.Json.Nodes;

static class Extensions
{
    public static void AssertEquality(this JsonNode? actual, JsonNode? expected)
    {
        if(expected is null)
        {
            if(actual is not null)
                fail($"Expected '{actual.GetPath()}' to be null.");

            return;
        }

        if(actual is null)
            fail($"Expected '{expected.GetPath()}' not to be null.");

        if(actual!.GetType() != expected.GetType())
            fail($"Expected '{actual.GetPath()}' to be '{expected.GetType().Name}' but found '{actual.GetType().Name}'.");

        if(expected is JsonObject expectedObj)
        {
            var actualObj = (JsonObject)actual;

            if(actualObj.Count != expectedObj.Count)
                fail($"Expected '{actual.GetPath()}' to have {expectedObj.Count} properties but found {actualObj.Count}.");

            foreach(var (name, expectedValue) in expectedObj)
            {
                if(!actualObj.TryGetPropertyValue(name, out var actualValue))
                    fail($"Expected '{actual.GetPath()}' to have '{name}' property");

                actualValue.AssertEquality(expectedValue);
            }
        } else if(expected is JsonArray expectedArr)
        {
            var actualArr = (JsonArray)actual;

            if(actualArr.Count != expectedArr.Count)
                fail($"Expected '{actual.GetPath()}' to have {expectedArr.Count} items but found {actualArr.Count}");

            var orderedExpectedArr = expectedArr.OrderBy(n => n?.ToString()).ToArray();
            var orderedActualArr = expectedArr.OrderBy(n => n?.ToString()).ToArray();

            for(var i = 0; i < orderedExpectedArr.Length; i++)
                orderedActualArr[i].AssertEquality(orderedExpectedArr[i]);
        } else
        {
            if(!JsonNode.DeepEquals(actual, expected))
                fail($"Expected '{actual.GetPath()}' ('{actual}') to equal '{expected.GetPath()}' ('{expected}')");
        }

        void fail(String message) =>
            Assert.Fail($"{message}:\nexpected:\n{expected}\nactual:\n{actual}");
    }
}