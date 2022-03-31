using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SimpleTiledWFC : BaseWFC{
	
	public TextAsset xml = null;
	private string subset = "";

	public int gridsize = 1;


	public int seed = 0;
	public bool periodic = false;
	public int iterations = 0;
	public bool incremental;

	public SimpleTiledModel model = null;
//	public GameObject[,] rendering;
	public GameObject output;
	private Transform group;
	public Dictionary<string, GameObject> obmap = new Dictionary<string, GameObject>();
    private bool undrawn = true;

	public void destroyChildren (){
		foreach (Transform child in this.transform) {
     		GameObject.DestroyImmediate(child.gameObject);
 		}
 	}

 	void Start(){
		Generate();
		Run();
	}

	void Update(){
		if (incremental){
			Run();
		}
	}


	public void Run(){
		if (model == null){return;}
        if (undrawn == false) { return; }
        if (model.Run(seed, iterations)){
			Draw();
		}
	}

    public override void SetSize(int w, int h, bool[,] fill)
    {
        width = w;
        depth = h;
        this.fill = fill;

		RunPreplacing();
    }

    private void RunPreplacing()
    {
        preplacement.Run(this);
    }

    public void OnDrawGizmos(){
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(new Vector3(width*gridsize/2f-gridsize*0.5f, depth*gridsize/2f-gridsize*0.5f, 0f),new Vector3(width*gridsize, depth*gridsize, gridsize));
	}

	public override void Generate(){
		obmap = new  Dictionary<string, GameObject>();

		if (output == null){
			Transform ot = transform.Find("output-tiled");
			if (ot != null){output = ot.gameObject;}}
		if (output == null){
			output = new GameObject("output-tiled");
			output.transform.parent = transform;
			output.transform.position = this.gameObject.transform.position;
			output.transform.rotation = this.gameObject.transform.rotation;}

		for (int i = 0; i < output.transform.childCount; i++){
			GameObject go = output.transform.GetChild(i).gameObject;
			if (Application.isPlaying){Destroy(go);} else {DestroyImmediate(go);}
		}
		group = new GameObject(xml.name).transform;
		group.parent = output.transform;
		group.position = output.transform.position;
		group.rotation = output.transform.rotation;
        group.localScale = new Vector3(1f, 1f, 1f);
        rendering = new GameObject[width, depth];
		this.model = new SimpleTiledModel(xml.text, subset, width, depth, periodic, predetermined);
        undrawn = true;
    }

	public void Draw(){
		if (output == null){return;}
		if (group == null){return;}
        undrawn = false;
		for (int y = 0; y < depth; y++){
			for (int x = 0; x < width; x++){ 
				if (rendering[x,y] == null){
                    if (IsRemoved(y * width + x))
                        continue;

                    string v = model.Sample(x, y);
					int rot = 0;
					GameObject fab = null;
					if (v != "?"){
						rot = int.Parse(v.Substring(0,1));
						v = v.Substring(1);
						if (!obmap.ContainsKey(v)){
							fab = (GameObject)Resources.Load(v, typeof(GameObject));
							obmap[v] = fab;
						} else {
							fab = obmap[v];
						}
						if (fab == null){
							continue;}
						Vector3 pos = new Vector3(x*gridsize, y*gridsize, 0f);
						GameObject tile = (GameObject)Instantiate(fab, new Vector3() , Quaternion.identity);
						Vector3 fscale = tile.transform.localScale;
						tile.transform.parent = group;
						tile.transform.localPosition = pos;
						tile.transform.localEulerAngles = new Vector3(0, 0, 360-(rot*90));
						tile.transform.localScale = fscale;
						rendering[x,y] = tile;
                    } else
                    {
                        undrawn = true;
                    }
				}
			}
  		}	
	}

    public override void Upscale(int scale)
    {
        int width = rendering.GetLength(0);
        int height = rendering.GetLength(1);

        GameObject[,] upscaled = new GameObject[width * scale, height * scale];

        var upGroup = new GameObject(xml.name).transform;
        upGroup.parent = output.transform;
        upGroup.position = output.transform.position;
        upGroup.rotation = output.transform.rotation;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                for (int i = 0; i < scale; i++)
                {
                    for (int j = 0; j < scale; j++)
                    {
                        var o = rendering[w, h] != null ? Instantiate(rendering[w, h], new Vector3(w * scale + i, h * scale + j, 0),
                            Quaternion.identity) : null;
                        upscaled[w * scale + i, h * scale + j] = o;
                        o.transform.parent = upGroup;
                    }
                }
            }
        }
        rendering = upscaled;
        if (Application.isPlaying) 
            Destroy(group.gameObject);
        else
            DestroyImmediate(group.gameObject);
        group = upGroup;
    }

	public override void AddBlocked(int index)
    {
        predetermined.Add(new Predetermined(index, new byte[]{0}));
    }
}

#if UNITY_EDITOR
[CustomEditor (typeof(SimpleTiledWFC))]
public class TileSetEditor : Editor {
	public override void OnInspectorGUI () {
		SimpleTiledWFC me = (SimpleTiledWFC)target;
		if (me.xml != null){
			if(GUILayout.Button("generate")){
				me.Generate();
			}
			if (me.model != null){
				if(GUILayout.Button("RUN")){
					me.model.Run(me.seed, me.iterations);
					me.Draw();
				}
			}
		}
		DrawDefaultInspector ();
	}
}
#endif