using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

public class JoinUIInputTests
{
	[UnityTest]
	public IEnumerator TestPrefabExists()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");
		
		// Make sure the prefab exists.
		Assert.NotNull(joinUI);

		yield return null;
	}

    [UnityTest]
	public IEnumerator TestNullAddress()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");
		
		// Test null.
		Assert.AreNotEqual("", joinUI.VerifyAddress(null));

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestWrongFormatAddress()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");

		// Test some invalid inputs containing incorrect formatting.
		Assert.AreNotEqual("", joinUI.VerifyAddress("0.0.0"));
		Assert.AreNotEqual("", joinUI.VerifyAddress("0.0.0.0.0"));
		Assert.AreNotEqual("", joinUI.VerifyAddress("...."));
		Assert.AreNotEqual("", joinUI.VerifyAddress("Hello"));

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestWordAddress()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");

		// Test some invalid inputs containing letters.
		Assert.AreNotEqual("", joinUI.VerifyAddress("a.b.c.d"));

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestTooLargeAddress()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");

		// Test some invalid inputs containing numbers >255.
		Assert.AreNotEqual("", joinUI.VerifyAddress("256.0.0.0"));
		Assert.AreNotEqual("", joinUI.VerifyAddress("0.256.0.0"));
		Assert.AreNotEqual("", joinUI.VerifyAddress("0.0.256.0"));
		Assert.AreNotEqual("", joinUI.VerifyAddress("0.0.0.256"));

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestCorrectAddress()
	{
		JoinServerUIController joinUI = AssetDatabase.LoadAssetAtPath<JoinServerUIController>("Assets/Prefabs/UI/Main Menu/Join Server.prefab");

		// Test a valid input.
		Assert.AreEqual("", joinUI.VerifyAddress("255.255.255.255"));

		yield return null;
	}
}
