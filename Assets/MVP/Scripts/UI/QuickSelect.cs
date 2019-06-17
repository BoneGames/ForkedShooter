using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSelect : MonoBehaviour 
{
	#region Variables
	[Header("Main UI")]
	public bool showSelectMenu;
	public bool toggleTogglable;
	public float scrW, scrH;
	[Header("Resources")]
	public Texture2D radialTexture;
	public Texture2D slotTexture;
	[Range(0,100)]
	public int circleScaleOffset;
	[Header("Icons")]
	public Vector2 iconSize;
	public bool showIcons, showBoxes, showBounds;
	[Range (0.1f,1)]
	public float iconSizeNum;
	[Range(-360,360)]
	public int radialRotation;
	[SerializeField]
	private float iconOffset;
	[Header("Mouse Settings")]
	public Vector2 mouse; // mousePos
	public Vector2 input; //mousePos
	private Vector2 circleCenter;
	[Header("Input Settings")]
	public float inputDist;
	public float inputAngle;
	public int keyIndex;
	public int mouseIndex;
	public int inputIndex;
	[Header("Sector Settings")]
	public Vector2[] slotPos;
	public Vector2[] boundPos;
	[Range(1,8)]
	public int numOfSectors = 1;
	[Range(50,300)]
	public float circleRadius;
	public float mouseDistance, sectorDegree, mouseAngles;
	public int sectorIndex = 0;
	public bool withinCircle;
	[Header("Misc")]
	private Rect debugWindow;
	#endregion

	void Start()
	{
		scrH = Screen.height / 9;
		scrW = Screen.width / 16;

		circleCenter.x = Screen.width / 2;
		circleCenter.y = Screen.height / 2;

		debugWindow = new Rect (Scr (0, 0), Scr (4, 1));
	}
	void Update()
	{
		if(Input.GetKey(KeyCode.Tab))
		{
			scrH = Screen.height / 9;
			scrW = Screen.width / 16;
			debugWindow = new Rect (Scr (0, 0), Scr (4, 1));

			showSelectMenu = true;
		}
		else if(Input.GetKeyUp(KeyCode.Tab))
		{
			showSelectMenu = false;
		}
	}
	void OnGUI()
	{
		debugWindow = GUI.Window (0, debugWindow, JoyStickUI, "");
		if(showSelectMenu)
		{
			CalculateMouseAngles ();

			sectorDegree = 360 / numOfSectors;
			iconOffset = sectorDegree / 2;
			slotPos = SlotPositions (numOfSectors);
			boundPos = BoundPositions (numOfSectors);
			//DeadZone
			GUI.Box (new Rect (Scr (7.5f, 4), Scr (1, 1)), "");
			//Circle
			GUI.DrawTexture(new Rect(circleCenter.x - circleRadius-(circleScaleOffset/4),circleCenter.y - circleRadius-(circleScaleOffset/4),(circleRadius*2)+(circleScaleOffset/2),(circleRadius*2)+(circleScaleOffset/2)),radialTexture);

			if(showBoxes)
			{
				for (int i = 0; i < numOfSectors; i++) 
				{
					GUI.DrawTexture (new Rect (	slotPos [i].x - (scrW * iconSizeNum * 0.5f), 
												slotPos [i].y - (scrH * iconSizeNum * 0.5f), 
												scrW * iconSizeNum, 
												scrH * iconSizeNum), slotTexture);
				}
			}
			if(showBounds)
			{
				for (int i = 0; i < numOfSectors; i++) 
				{
					GUI.Box (new Rect (	boundPos [i].x - (scrW * 0.1f * 0.5f), 
										boundPos [i].y - (scrH * 0.1f * 0.5f), 
										scrW * 0.1f, scrH * 0.1f), "");
				}
			}
			if(showIcons)
			{
				SetItemSlots (numOfSectors, slotPos);
			}
		}
	}
	void CalculateMouseAngles()
	{
		mouse = Input.mousePosition;
		input.x = Input.GetAxis ("Horizontal");
		input.y = -Input.GetAxis ("Vertical");

		//Distance 
		mouseDistance = Mathf.Sqrt(Mathf.Pow((mouse.x - circleCenter.x), 2) + 
						Mathf.Pow((mouse.y - circleCenter.y),2));

		inputDist = Vector2.Distance(Vector2.zero, input);

		withinCircle = mouseDistance <= circleRadius ? true : false;
		if(input.x != 0 || input.y != 0)
		{
			inputAngle = (Mathf.Atan2 (-input.y, input.x) * 180 / Mathf.PI) 
							+ radialRotation;
		}
		else
		{
			mouseAngles = (Mathf.Atan2 (mouse.y - circleCenter.y, mouse.x - circleCenter.x) * 180 / Mathf.PI)
							+ radialRotation;
		}
		if(mouseAngles < 0)
		{
			mouseAngles += 360;
		}
		if(inputAngle < 0)
		{
			inputAngle += 360;
		}
		inputIndex = CheckCurrentSector (inputAngle);
		mouseIndex = CheckCurrentSector (mouseAngles);
		if(input.x != 0 || input.y != 0)
		{
			sectorIndex = inputIndex;
		}
		if(input.x == 0 && input.y == 0)
		{
			if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
			{
				sectorIndex = mouseIndex;
			}
		}
	}
	private int CheckCurrentSector(float angle)
	{
		float boundingAngle = 0;
		for (int i = 0; i < numOfSectors; i++) 
		{
			boundingAngle += sectorDegree;
			if(angle < boundingAngle)
			{
				return i;
			}
		}
		return 0;
	}
	void SetItemSlots(int slots, Vector2[] pos)
	{
		for (int i = 0; i < slots; i++) 
		{
			GUI.DrawTexture (new Rect ( pos [i].x - (scrW * iconSizeNum * 0.5f), 
										pos [i].y - (scrH * iconSizeNum * 0.5f), 
										scrW * iconSizeNum, 
										scrH * iconSizeNum),slotTexture);
		}
	}
	private Vector2[] SlotPositions(int slots)
	{
		Vector2[] slotPos = new Vector2[slots];
		float angle = ((iconOffset / 2) * 2) + radialRotation;
		for (int i = 0; i < slotPos.Length; i++)
		{
			slotPos [i].x = circleCenter.x + circleRadius * Mathf.Cos (angle * Mathf.Deg2Rad);
			slotPos [i].y = circleCenter.y + circleRadius * Mathf.Sin (angle * Mathf.Deg2Rad);
			angle += sectorDegree;
		}
		return slotPos;
	}
	private Vector2[] BoundPositions(int slots)
	{
		Vector2[] boundPos = new Vector2[slots];
		float angle = 0 + radialRotation;
		for (int i = 0; i < boundPos.Length; i++) 
		{
			boundPos [i].x = circleCenter.x + circleRadius * Mathf.Cos (angle * Mathf.Deg2Rad);
			boundPos [i].y = circleCenter.y + circleRadius * Mathf.Sin (angle * Mathf.Deg2Rad);
			angle += sectorDegree;
		}
		return  boundPos;
	}
	void JoyStickUI(int windowID)
	{
		GUI.Box (new Rect (Scr (0, 0), Scr (1, 1)), "");

		GUI.Box(new Rect(Scr(0.25f + (Input.GetAxis("Horizontal")*0.25f),
			0.25f + (-Input.GetAxis("Vertical")*0.25f)),
			Scr(0.5f,0.5f)),"");
		GUI.Box (new Rect (Scr(1.25f,0.25f),Scr(0.5f,0.5f)),"Tab");
		if(showSelectMenu)
		{
			GUI.Box (new Rect (Scr(1.25f,0.25f),Scr(0.5f,0.5f)),"");
		}
		GUI.DragWindow ();
	}
	private Vector2 Scr(float x, float y)
	{
		Vector2 coord = Vector2.zero;
		coord = new Vector2 (scrW * x, scrH * y);
		return coord;
	}
}
