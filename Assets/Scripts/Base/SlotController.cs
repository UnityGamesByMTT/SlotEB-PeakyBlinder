using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using Newtonsoft.Json;

public class SlotController : MonoBehaviour
{


    [Header("Animation Sprites")]
    [SerializeField] internal Sprite[] iconImages;
    [SerializeField] private List<Sprite> ID_0_8;
    [SerializeField] private List<Sprite> ID_13_;
    [SerializeField] private List<Sprite> ID_9;
    [SerializeField] private List<Sprite> ID_10;
    [SerializeField] private List<Sprite> ID_11;
    [SerializeField] private List<Sprite> ID_12;

    [Header("Slot Images")]
    [SerializeField] internal List<SlotImage> slotMatrix;
    [SerializeField] internal GameObject disableIconsPanel;


    [Header("Slots Transforms")]
    [SerializeField] private RectTransform[] Slot_Transform;
    [SerializeField] private RectTransform mask_transform;
    [SerializeField] private RectTransform bg_mask_transform;
    [SerializeField] private RectTransform[] bg_slot_transform;
    [SerializeField] private RectTransform[] sideBars;
    [SerializeField] private ImageAnimation[] sideBarsAnim;

    [SerializeField] private RectTransform[] horizontalBars;
    [SerializeField] internal ImageAnimation watchAnimation;
    [SerializeField] internal int level;

    [SerializeField] private TMP_Text noOfWays;

    [Header("tween properties")]
    [SerializeField] private float tweenHeight = 0;
    [SerializeField] private float initialPos;

    [SerializeField] private GameObject[] border;

    private List<Tweener> alltweens = new List<Tweener>();

    [SerializeField] internal List<SlotIconView> animatingIcons = new List<SlotIconView>();

    internal IEnumerator StartSpin(bool turboMode, bool immediateStop)
    {

        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            RespectMask(i);
            InitializeTweening(Slot_Transform[i], turboMode);
            if (!GameManager.ImmediateStop)
                yield return new WaitForSeconds(0.15f);


        }
    }

    internal void PopulateSLotMatrix(List<List<int>> resultData, List<List<double>> coins = null)
    {


            if (coins != null)
            {
                for (int i = 0; i < coins.Count; i++)
                {
                    if (coins[i][3] == 13)
                        slotMatrix[(int)coins[i][0]].slotImages[(int)coins[i][1]].SetCoin(coins[i][2]);
                }
            }


        for (int i = 0; i < resultData.Count; i++)
        {
            for (int j = 0; j < resultData[i].Count; j++)
            {
                slotMatrix[j].slotImages[i].SetIcon(iconImages[resultData[i][j]], resultData[i][j], (i * 10 + j));
            }
        }
    }

    internal void RespectMask(int i)
    {

        for (int j = 0; j < slotMatrix[i].slotImages.Count; j++)
        {
            slotMatrix[i].slotImages[j].RespectMask();
        }
        // matrixRowCount++;
    }


    private void IgnoreMask(int i)
    {

        for (int j = 0; j < slotMatrix[i].slotImages.Count; j++)
        {
            if (slotMatrix[i].slotImages[j].id > 8 && slotMatrix[i].slotImages[j].id < 13)
                slotMatrix[i].slotImages[j].Ignoremask();
        }
        // matrixRowCount++;

    }
    internal IEnumerator StopSpin(bool ignore = true, Action playStopSound = null, bool isFreeSpin = false, bool turboMode = false)
    {
        GameObject activeBorder = null;

        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            if (activeBorder != null)
                activeBorder.SetActive(false);

            // if (!GameManager.ImmediateStop)
            //     playStopSound?.Invoke();
            StopTweening(Slot_Transform[i], i, GameManager.ImmediateStop, turboMode, playStopSound);

            if (ignore)
                IgnoreMask(i);

            if (!isFreeSpin && !GameManager.ImmediateStop && !turboMode)
            {
                for (int j = 0; j < SocketModel.resultGameData.ResultReel.Count; j++)
                {
                    if (SocketModel.resultGameData.ResultReel[j][i] >= 13)
                    {

                        if (i + 1 < border.Length)
                        {
                            Debug.Log("eneterd");
                            border[i + 1].transform.localScale = new Vector3(1, 0, 1);
                            border[i + 1].SetActive(true);
                            border[i + 1].transform.DOScaleY(1, 0.2f);
                            activeBorder = border[i + 1];
                            yield return new WaitForSeconds(1f);
                            break;
                        }


                    }
                }

            }

            if (!GameManager.ImmediateStop)
            {

                if (turboMode)
                    yield return new WaitForSeconds(0.1f);
                else
                    yield return new WaitForSeconds(0.2f);
            }


        }

        if (GameManager.ImmediateStop && !GameManager.thunderFreeSpins)
        {
            playStopSound?.Invoke();
            yield return new WaitForSeconds(0.15f);
        }


        KillAllTweens();

    }

    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < slotMatrix.Count; i++)
        {
            for (int j = 0; j < slotMatrix[i].slotImages.Count; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 8);
                slotMatrix[i].slotImages[j].SetIcon(iconImages[randomIndex], randomIndex, (i * 10 + j));
            }
        }
    }



    internal void StartIconAnimation(List<string> iconPos, Transform paylineSymbolAnimPanel)
    {
        SlotIconView tempIcon;
        for (int j = 0; j < iconPos.Count; j++)
        {
            int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            tempIcon = slotMatrix[pos[0]].slotImages[pos[1]];

            if (tempIcon.id == 9)
                tempIcon.StartAnim(ID_9, tempIcon.id);
            else if (tempIcon.id == 10)
                tempIcon.StartAnim(ID_10, tempIcon.id);
            else if (tempIcon.id == 11)
                tempIcon.StartAnim(ID_11, tempIcon.id);
            else if (tempIcon.id == 12)
                tempIcon.StartAnim(ID_12, tempIcon.id);
            else if(tempIcon.id>=13)
                tempIcon.StartAnim(ID_13_, tempIcon.id);
            else
                tempIcon.StartAnim(ID_0_8, tempIcon.id);

            animatingIcons.Add(tempIcon);
            tempIcon.SetParent(paylineSymbolAnimPanel);
        }

    }
    internal void StartIconAnimation(List<List<int>> iconPos, Transform paylineSymbolAnimPanel)
    {
        SlotIconView tempIcon;
        for (int j = 0; j < iconPos.Count; j++)
        {
            tempIcon = slotMatrix[iconPos[j][0]].slotImages[iconPos[j][1]];

            if (tempIcon.id == 9)
                tempIcon.StartAnim(ID_9, tempIcon.id);
            else if (tempIcon.id == 10)
                tempIcon.StartAnim(ID_10, tempIcon.id);
            else if (tempIcon.id == 11)
                tempIcon.StartAnim(ID_11, tempIcon.id);
            else if (tempIcon.id == 12)
                tempIcon.StartAnim(ID_12, tempIcon.id);
            else if(tempIcon.id>=13)
                tempIcon.StartAnim(ID_13_, tempIcon.id);
            else
                tempIcon.StartAnim(ID_0_8, tempIcon.id);

            animatingIcons.Add(tempIcon);
            tempIcon.SetParent(paylineSymbolAnimPanel);
        }

    }

    internal void StopIconAnimation()
    {

        for (int i = 0; i < animatingIcons.Count; i++)
        {
            animatingIcons[i].StopAnim();
            // if(animatingIcons[i].id>=13)
            // animatingIcons[i].activeanimation.gameObject.SetActive(false);
        }
        animatingIcons.Clear();

    }

    internal void PlaySymbolAnim(List<string> iconPos, Transform paylineSymbolAnimPanel)
    {

        for (int i = 0; i < iconPos.Count; i++)
        {

            int[] pos = iconPos[i].Split(',').Select(int.Parse).ToArray();
            slotMatrix[pos[0]].slotImages[pos[1]].SetParent(paylineSymbolAnimPanel);

            // if (!animatingIcons.Any(x => x.pos == slotMatrix[pos[0]].slotImages[pos[1]].pos))
            //     animatingIcons.Add(slotMatrix[pos[0]].slotImages[pos[1]]);
        }

    }
    internal void StopSymbolAnim(List<string> iconPos)
    {

        for (int i = 0; i < iconPos.Count; i++)
        {
            int[] pos = iconPos[i].Split(',').Select(int.Parse).ToArray();
            slotMatrix[pos[0]].slotImages[pos[1]].ResetParent();
        }

    }

    internal void ResetAllSymbols()
    {

        for (int i = 0; i < slotMatrix.Count; i++)
        {
            for (int j = 0; j < slotMatrix[i].slotImages.Count; j++)
            {
                slotMatrix[i].slotImages[j].ResetParent();
                slotMatrix[i].slotImages[j].DeActivateIconBorder();
                //  slotMatrix[i].slotImages[j].ClearCoin();

            }

        }
        // animatingIcons.Clear();

    }
    internal void CLearAllCoins()
    {

        for (int i = 0; i < slotMatrix.Count; i++)
        {
            for (int j = 0; j < slotMatrix[i].slotImages.Count; j++)
            {

                slotMatrix[i].slotImages[j].ClearCoin();
            }

        }
        // animatingIcons.Clear();

    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform, bool turboMode)
    {
        float delay = 0.4f;
        if (turboMode)
            delay = 0.2f;
        // if(immediateStop)
        // delay=0.05f;
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, delay).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.Linear);
        alltweens.Add(tweener);
    }

    private void StopTweening(Transform slotTransform, int index, bool immediateStop, bool turboMode, Action StopSpinAudioAction)
    {

        float delay = 0.2f;

        if (turboMode)
            delay = 0.1f;

        if (immediateStop)
            delay = 0.05f;

        alltweens[index].Pause();
        
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, initialPos + 265);
        
        alltweens[index] = slotTransform.DOLocalMoveY(initialPos, delay).SetEase(Ease.OutFlash).OnComplete(()=>{
            if(!immediateStop && !GameManager.thunderFreeSpins){
                StopSpinAudioAction?.Invoke();
            }
        });
    }
    private void KillAllTweens()
    {
        for (int i = 0; i < alltweens.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{

    public List<SlotIconView> slotImages = new List<SlotIconView>(10);
}


