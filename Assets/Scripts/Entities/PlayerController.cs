using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public void Update()
	{
		float xOffset = 0;
		float yOffset = 0;

		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			yOffset += 0.01f;
		}
		if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			yOffset -= 0.01f;
		}
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			xOffset -= 0.01f;
		}
		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			xOffset += 0.01f;
		}

		this.transform.position += new Vector3(xOffset, yOffset, 0);
	}
}