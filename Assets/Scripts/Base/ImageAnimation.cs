﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
	public enum ImageState
	{
		NONE,
		PLAYING,
		PAUSED
	}
	public static ImageAnimation Instance;

	public List<Sprite> textureArray;

	public Image rendererDelegate;

	public bool useSharedMaterial = true;

	public bool doLoopAnimation = true;

	[HideInInspector]
	public ImageState currentAnimationState;

	private int indexOfTexture;

	private float idealFrameRate = 0.0416666679f;

	private float delayBetweenAnimation;

	public float AnimationSpeed = 5f;

	public float delayBetweenLoop;

	public bool StartOnAwake = false;

	public bool DestroyOnCompletion = false;

	[SerializeField] internal bool isplaying;

	[SerializeField]
	private Sprite OriginalSprite;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

    private void Start()
    {
    }

    private void OnEnable()
	{
		if (StartOnAwake)
        {
			StartAnimation();
        }
	}

	private void OnDisable()
	{
		//rendererDelegate.sprite = textureArray[0];
		StopAnimation();
	}

	private void AnimationProcess()
	{
		isplaying = true;
		SetTextureOfIndex();
		indexOfTexture++;
		if (indexOfTexture == textureArray.Count)
		{
			indexOfTexture = 0;
			if (doLoopAnimation)
			{
				Invoke("AnimationProcess", delayBetweenAnimation + delayBetweenLoop);
				isplaying = true;
			}
			else
            {
				if(DestroyOnCompletion)
                {
					this.gameObject.SetActive(false);
                }
				isplaying = false;
			}
		}
		else
		{
			Invoke("AnimationProcess", delayBetweenAnimation);
			isplaying = true;

		}
	}

	public void StartAnimation()
	{
		indexOfTexture = 0;
		if (currentAnimationState == ImageState.NONE)
		{
			RevertToInitialState();
			delayBetweenAnimation = idealFrameRate * (float)textureArray.Count / AnimationSpeed;
			currentAnimationState = ImageState.PLAYING;
			Invoke("AnimationProcess", delayBetweenAnimation);
		}
	}

	public void PauseAnimation()
	{
		if (currentAnimationState == ImageState.PLAYING)
		{
			CancelInvoke("AnimationProcess");
			currentAnimationState = ImageState.PAUSED;
		}
	}

	public void ResumeAnimation()
	{
		if (currentAnimationState == ImageState.PAUSED && !IsInvoking("AnimationProcess"))
		{
			Invoke("AnimationProcess", delayBetweenAnimation);
			currentAnimationState = ImageState.PLAYING;
		}
	}

	public void StopAnimation()
	{
        if (currentAnimationState != 0)
        {
			try
			{
				           if (OriginalSprite != null)
                rendererDelegate.sprite = OriginalSprite;
            else
                rendererDelegate.sprite = textureArray[0];
			}
			catch (System.Exception)
			{
				
				Debug.Log(" texture array"+textureArray.Count);
			}
			try
			{
			CancelInvoke("AnimationProcess");
				
			}
			catch (System.Exception)
			{
				
				Debug.Log("error in cancelling invoke");
			}
			currentAnimationState = ImageState.NONE;
			isplaying = false;
		}
}

	public void RevertToInitialState()
	{
		indexOfTexture = 0;
		SetTextureOfIndex();
	}

	private void SetTextureOfIndex()
	{
		if (useSharedMaterial)
		{
			rendererDelegate.sprite = textureArray[indexOfTexture];
		}
		else
		{
			rendererDelegate.sprite = textureArray[indexOfTexture];
		}
	}
}
