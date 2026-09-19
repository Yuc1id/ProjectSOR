using UnityEngine;

public class Object_VectorPlate : MonoBehaviour
{
	public bool isField = true;
	public float addSpeedStrength = 5f;
	public Vector2 addSpeedDirection = Vector2.zero;
	[HideInInspector]public Vector2 addSpeed;

	void OnEnable()
	{
		addSpeed = addSpeedStrength * addSpeedDirection;
	}
}
