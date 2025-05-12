using UnityEngine;

public class UIBase : MonoBehaviour
{
	/// <summary>The previous menu.</summary>
	protected UIBase prevUI = null;

	/// <summary>
	/// Store the previous menu and deactivate it.
	/// </summary>
	/// <param name="previousUI"></param>
	public void Open(UIBase previousUI)
	{
		this.prevUI = previousUI;
		this.prevUI.gameObject.SetActive(false);
	}

	/// <summary>
	/// Close the menu entirely, including all previous menus.
	/// </summary>
	public void Close()
	{
		if(this.prevUI != null)
		{
			this.prevUI.Close();
		}

		Destroy(this.gameObject);
	}

	/// <summary>
	/// Close the menu and reactivate the previous one.
	/// </summary>
	public void Back()
	{
		if(this.prevUI != null)
		{
			this.prevUI.gameObject.SetActive(true);
		}

		Destroy(this.gameObject);
	}
}