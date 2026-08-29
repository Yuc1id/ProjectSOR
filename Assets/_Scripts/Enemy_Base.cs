using UnityEngine;

namespace ProjectSOR
{
	public abstract class Enemy_Base : MonoBehaviour
	{
		//Constant
		[SerializeField] protected int maxHP = 10;
		//Variable
		protected float hp;


		public virtual void ReceiveDamage(float damage)
		{
			hp -= damage;
			Debug.Log($"{gameObject.name} received {damage} damage. Current HP: {hp}");
		}

		protected virtual void Start()
		{
			hp = maxHP;
		}
	}
}