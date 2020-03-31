using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
    public static List<Cell> spikeCells = new List<Cell>();
	TextMesh text;
    TextMesh textAprox;
    public enum StateCell {Normal, Passed, EndCell};
	public StateCell stateCell = StateCell.Normal;
	public int number = 3;
	Color32 defaultColor;
	public bool operation = false;
    public bool spikes = false;
    public float spikeTime = 2.5f;
    bool approx = false;
    private TweenPosition [] spikesTP;
    public AudioClip audioClip;
    private IEnumerator spikesIE;
    private bool spikePaused = false;
	// Use this for initialization

    void Awake(){
        text = transform.Find("Text").GetComponent<TextMesh>();
        text.font = InGame.Instance.cellFont;
        text.GetComponent<MeshRenderer>().material = InGame.Instance.cellTextMaterials[0];
    }

	void Start () {
        SetNumber(number);
		changeState (stateCell);
		defaultColor = GetComponent<Renderer> ().material.GetColor ("_EmissionColor");
        if(spikes){
            //Debug.Log("init spike");
            spikeCells.Add(this);
            spikesTP = GetComponentsInChildren<TweenPosition>();
            foreach(TweenPosition tp in spikesTP){
                tp.duration = spikeTime;
                tp.enabled = false;
            }
            spikesIE = PlaySpike();
            StartCoroutine(spikesIE);
        }
    }

    private IEnumerator PlaySpike(float delay = 0){
        //Debug.Log("Spikes: "+spikeCells.Count);
        yield return new WaitForSeconds(delay);
        if(spikeCells[0] == this || spikePaused)
            InGame.Instance.PlayFX(audioClip,0.75f);
        foreach(TweenPosition tp in spikesTP){
            tp.ResetToBeginning();
            tp.PlayForward();
        }
        yield return new WaitForSeconds(spikeTime);
        spikesIE = PlaySpike();
        StartCoroutine(spikesIE);
    }


    public void SetNumberApprox(int number)
    {
        approx = true;
        SetNumber(number);
    }

    public void SetNumber(int number)
    {
        this.number = number;
        SetText("" + number);
        if (approx)
            CreateAprox();
    }

    public void CreateAprox()
    {
        GameObject textAproxGO = Instantiate(text.gameObject, text.gameObject.transform.position, text.gameObject.transform.rotation);
        textAproxGO.name = "TextApprox";
        textAproxGO.transform.parent = transform;
        textAproxGO.transform.localPosition += new Vector3(0f, 0f, -0.3f);
        textAprox = textAproxGO.GetComponent<TextMesh>();
        textAprox.fontSize = 26;
        textAprox.text = "Approx.";
    }

	public void changeState(StateCell s){
		switch (s) {
		case StateCell.Passed:
            SetText("-");
			GetComponent<MeshRenderer> ().enabled = false;
            //elimina el script para cambio de numero automatico
            AutomaticChangeNumber a = GetComponent<AutomaticChangeNumber>();
            if (a != null) Destroy(a);
            //elimina el script para cambio de numero invisible
            DissapearNumber d = GetComponent<DissapearNumber>();
            if (d != null) Destroy(d);
            break;
		}
		stateCell = s;
	}

    public void SetText(string text)
    {
        if (textAprox != null) Destroy(textAprox.gameObject);
        this.text.text = text;
    }

    public void EnableSpikes(bool b){
        if(b){
            //spikeCells.Add(this);
            spikesIE = PlaySpike(1f);
            StartCoroutine(spikesIE);
        }
        else{
            spikeCells.Remove(this);
            spikePaused = true;
            if(spikesIE != null)
                StopCoroutine(spikesIE);
        }
    }

	public IEnumerator shine(int num){
		Material m = GetComponent<Renderer> ().material;
		Color32 colorDefault = m.GetColor ("_EmissionColor");
		for (int j = 0; j < num; j++) {
			for (int i = 0; i < 15; i++) {
				yield return new WaitForSeconds (0.01f);
				m.SetColor ("_EmissionColor", Color.yellow * i * 0.1f);
			}
			for (int i = 15; i > 0; i--) {
				yield return new WaitForSeconds (0.01f);
				m.SetColor ("_EmissionColor", Color.yellow * i * 0.1f);
			}
		}
		m.SetColor ("_EmissionColor", colorDefault);
	}

	public void unshine(){
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", defaultColor);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init(int n){
		number = n;
		text = transform.Find ("Text").GetComponent<TextMesh> ();
		text.text = "" + number;
	}
}
