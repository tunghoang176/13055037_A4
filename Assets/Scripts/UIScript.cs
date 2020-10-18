using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

	[SerializeField]
	private Text txt_score = null;
	[SerializeField]
	private Text txt_hightScore = null;
	[SerializeField]
	private Text txt_level = null;

	public List<Image> lives = new List<Image>(3);
	
	// Use this for initialization
	void Start () 
	{
	    for (int i = 0; i < 3 - GameManager.lives; i++)
	    {
	        Destroy(lives[lives.Count-1]);
            lives.RemoveAt(lives.Count-1);
	    }
		if(txt_hightScore) txt_hightScore.text = "High Score\n" + GameManager.hightScore;
		if(txt_level) txt_level.text = "Level\n" + GameManager.Level;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(txt_score) txt_score.text = "Score\n" + GameManager.score;
	}
}
