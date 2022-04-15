﻿using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public bool upDown;
	public bool sideSide;
	private float timer;
	private int state = 0; //0 - move1, 1 - pause, 2- move2, 3 - pause2
	public float waitTime = 2;
	public float time = 2;
	public float speed = 2;
	private Rigidbody rb;

	void Start()
	{
		state = 0;
		timer = 0;
		rb = GetComponent<Rigidbody>();
	}

    private void OnCollisionStay(Collision collision)
    {
        if (Vector3.Dot(collision.GetContact(0).normal, Vector3.down) > 0.9 && sideSide)
        {
			collision.gameObject.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
	{
		if (sideSide == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				rb.MovePosition(transform.position - transform.forward * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}

		if (upDown == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				rb.MovePosition(transform.position + Vector3.up * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				rb.MovePosition(transform.position + Vector3.down * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}

	}
}
