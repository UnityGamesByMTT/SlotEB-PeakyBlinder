using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PollyFreeSpinController : MonoBehaviour
{
    [SerializeField] RectTransform border;
    [SerializeField] float origin;
    [SerializeField] ImageAnimation[] slotAnim;

    public int noOfColumns;

    Vector3 originalPos;
    internal Func<Action, Action, bool, bool, float, float, IEnumerator> SpinRoutine;

    Coroutine spin;

    internal Action<int, double> UpdateUI;
    internal Action<int, GameObject> FreeSpinPopUP;
    internal Action<GameObject> FreeSpinPopUpClose;
    internal Action FreeSpinPopUPOverlay;
    [SerializeField] GameObject pollySpinBg;

    [SerializeField] internal ThunderFreeSpinController thunderFP;

    private void Start()
    {
        originalPos = border.transform.localPosition;
    }

    internal IEnumerator StartFP(int count)
    {
        FreeSpinPopUPOverlay?.Invoke();
        yield return new WaitWhile(()=>!UIManager.freeSpinOverLayOpen);

        FreeSpinPopUP?.Invoke(count, pollySpinBg);
        yield return new WaitForSeconds(1.8f);
        FreeSpinPopUpClose?.Invoke(pollySpinBg);

        border.parent.gameObject.SetActive(true);


        while (count > 0)
        {
            count--;
            UpdateUI?.Invoke(count, -1);
            yield return spin = StartCoroutine(SpinRoutine(SetBorder, ReSetBorder, false, false, 0, 0f));
            UpdateUI?.Invoke(-1, SocketModel.playerData.currentWining);
            if (SocketModel.resultGameData.freeSpinAdded)
            {
                if (spin != null)
                    StopCoroutine(spin);
                int prevFreeSpin = count;
                count = SocketModel.resultGameData.freeSpinCount;
                int freeSpinAdded = count - prevFreeSpin;
                UpdateUI?.Invoke(count, -1);
                FreeSpinPopUP?.Invoke(freeSpinAdded, null);
                yield return new WaitForSeconds(1.5f);
                FreeSpinPopUpClose?.Invoke(null);


            }

            if (SocketModel.resultGameData.thunderSpinCount > 0)
            {
                if (spin != null)
                    StopCoroutine(spin);
                border.parent.gameObject.SetActive(false);
                yield return thunderFP.StartFP(
                froxenIndeces: SocketModel.resultGameData.frozenIndices,
                count: SocketModel.resultGameData.thunderSpinCount,
                ResultReel: SocketModel.resultGameData.ResultReel
                );
                border.parent.gameObject.SetActive(true);

            }

            if(SocketModel.playerData.currentWining>0)
            yield return new WaitForSeconds(3f);
            else
            yield return new WaitForSeconds(1f);

        }
        border.parent.gameObject.SetActive(false);

    }
    internal void SetBorder()
    {
        int colIndex = FindColIndex();
        if (colIndex < 0)
            return;

        border.transform.localPosition = originalPos;
        border.transform.localPosition += new Vector3((noOfColumns - 1) * 132 + colIndex * 267.5f, 0);
        border.sizeDelta = new Vector2(274 * noOfColumns + (63 * 2), 820);
        border.localScale = new Vector3(1, 0.02f, 1);
        border.gameObject.SetActive(true);
        for (int i = colIndex; i < colIndex + noOfColumns; i++)
        {
            int index = i;
            slotAnim[i].transform.localScale = new Vector3(1, 0.02f, 1);
            slotAnim[i].gameObject.SetActive(true);
            slotAnim[i].transform.DOScaleY(1, 0.2f).OnComplete(slotAnim[index].StartAnimation);
        }
        border.DOScaleY(1, 0.2f).SetEase(Ease.InOutExpo);
    }
    internal void ReSetBorder()
    {
        border.transform.localPosition = originalPos;
        border.sizeDelta = new Vector2(400, 820);
        border.localScale = new Vector3(1, 1, 1);
        border.gameObject.SetActive(false);
        for (int i = 0; i < slotAnim.Length; i++)
        {
            slotAnim[i].transform.localScale = new Vector3(1, 1f, 1);
            slotAnim[i].gameObject.SetActive(false);
        }
    }

    private int FindColIndex()
    {
        int index = -1;

        List<string> convertedmatrix = Helper.Convert2dToLinearMatrix(SocketModel.resultGameData.ResultReel);

        for (int i = 0; i < convertedmatrix.Count; i++)
        {
            if (i + 2 < convertedmatrix.Count)
            {

                if (convertedmatrix[i] == convertedmatrix[i + 1] && convertedmatrix[i + 1] == convertedmatrix[i + 2])
                {
                    index = i;
                    break;
                }

            }
        }
        return index;


    }
}
