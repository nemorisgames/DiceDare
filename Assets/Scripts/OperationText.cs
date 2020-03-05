using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class OperationText : MonoBehaviour
{
    public Color correctColor;
    public Color wrongColor;
    public Text num1, num2, res, op, line, resto;
    private Text [] allTexts;
    public Image background;
    private Color currentColor;
    private CanvasGroup canvasGroup;
    private IEnumerator display;
    private float pauseTime = 0.15f;
    private float fadeInStep = 0.2f;
    private bool hasRemainder = false;

    void Start()
    {
        allTexts = GetComponentsInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        currentColor = correctColor;
        currentColor.a = 0;
        Color aux = background.color;
        aux.a = 0;
        background.color = aux;

        foreach(Text t in allTexts){
            t.color = currentColor;
        }
    }

    public void Init(bool correct, int n1, int n2, int r, string operation, int _resto){
        num1.text = n1.ToString();
        num2.text = n2.ToString();
        res.text = r.ToString();
        if(_resto > 0){
            hasRemainder = true;
            resto.text = "R: "+_resto.ToString();
            background.rectTransform.localPosition = new Vector2(0f,1.2f);
            background.rectTransform.sizeDelta = new Vector2(1.35f,3f);
        }
        else{
            resto.text = "";
        }
        op.text = operation;
        if(correct){
            currentColor = correctColor;
        }
        else{
            currentColor = wrongColor;
        }

        currentColor.a = 0;

        if(display != null)
            StopCoroutine(display);
        display = Display(true,correct);
        StartCoroutine(display);
    }

    public void DimOperation(float alpha){
        Color aux;
        foreach(Text t in allTexts){
            aux = t.color;
            aux.r -= alpha;
            aux.b -= alpha;
            aux.g -= alpha;
            t.color = aux;
        }
        currentColor.r -= alpha;
        currentColor.b -= alpha;
        currentColor.g -= alpha;
        StartCoroutine(Resize(0.8f));
    }

    IEnumerator Resize(float magnitude){
        Vector3 position = transform.position;
        Vector3 target = transform.localScale * magnitude;
        while(transform.localScale.x > magnitude){
            transform.localScale -= Vector3.one * 0.05f;
            transform.position = position;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.localScale = target;
    }

    IEnumerator Display(bool dir, bool b){
        if(dir){
            Color aux = background.color;
            aux.a = 0;
            while(aux.a < 0.8f){
                aux.a += fadeInStep / 2f;
                background.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            currentColor = (b ? correctColor : wrongColor);

            aux = currentColor;
            aux.a = 0;
            while(aux.a < 1){
                aux.a += fadeInStep;
                num1.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(pauseTime);
            aux = currentColor;
            aux.a = 0;
            while(aux.a < 1){
                aux.a += fadeInStep;
                op.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            aux = currentColor;
            aux.a = 0;
            while(aux.a < 1){
                aux.a += fadeInStep;
                num2.color = aux;
                //line.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(pauseTime);
            aux = currentColor;
            aux.a = 0;

            while(aux.a < 1){
                aux.a += fadeInStep;
                line.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(pauseTime);
            aux = currentColor;
            aux.a = 0;
            
            while(aux.a < 1){
                aux.a += fadeInStep;
                res.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            aux = currentColor;
            aux.a = 0;

            if(hasRemainder){
                while(aux.a < 1){
                aux.a += fadeInStep;
                resto.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
                }
                aux = currentColor;
                aux.a = 0;
            }
            
            currentColor.a = 1;

            yield return new WaitForSeconds(1.5f);
            if(display != null)
            StopCoroutine(display);
                display = Display(false,b);
            StartCoroutine(display);
        }
        else{
            currentColor = num1.color;
            while(currentColor.a > 0){
                currentColor.a -= fadeInStep;
                foreach(Text t in allTexts){
                    t.color = currentColor;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }

            Color aux = background.color;
            while(aux.a > 0){
                aux.a -= fadeInStep;
                background.color = aux;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            ShowOperation.Instance.RemoveOperationText(this);
        }
    }
}
