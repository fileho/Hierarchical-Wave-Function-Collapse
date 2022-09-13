using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class OverlapWFC : BaseWFC{
	public Training training = null;
    public int gridsize = 1;
    public int seed = 0;
    public int N = 3;
	public bool periodicInput = true;
	public bool periodicOutput = true;
	public int symmetry = 1;
	[HideInInspector]
	public int foundation = 0;
	public int iterations = 0;
	public bool incremental = true;
	public OverlappingModel model = null;
	public bool autoTile = false;
//	public GameObject[,] rendering;
    [HideInInspector]
	public GameObject output;
	private Transform group;
    private bool undrawn = true;

    public static bool IsPrefabRef(UnityEngine.Object o){
		#if UNITY_EDITOR
		return PrefabUtility.GetOutermostPrefabInstanceRoot(o) != null;
		#else
		return true;
		#endif
	}

	static GameObject CreatePrefab(UnityEngine.Object fab, Vector3 pos, Quaternion rot) {
		#if UNITY_EDITOR
		GameObject e = PrefabUtility.InstantiatePrefab(fab as GameObject) as GameObject; 
		e.transform.SetPositionAndRotation(pos, rot);
        return e;
		#else
		GameObject o = GameObject.Instantiate(fab as GameObject) as GameObject; 
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
		#endif
	}

	public void Clear(){
		if (group != null){
			if (Application.isPlaying){Destroy(group.gameObject);} else {
				DestroyImmediate(group.gameObject);
			}	
			group = null;
		}
	}

    void Update(){
		if (incremental){
			Run();
		}
	}

    public override void SetSize(int w, int h, bool[,] fill)
    {
        if (Application.isPlaying)
            iterations = 200;

        width = w;
        depth = h;
        this.fill = fill;

        if (!periodicOutput)
        {
            width = w + N - 1;
            depth = h + N - 1;
		}

        debug = false;
        RunPreplacing();
    }

    public override void Generate()
    {
        if (debug)
        {
            fill = GetPreplacement();
			RunPreplacing();
        }

        if (training == null){Debug.Log("Can't Generate: no designated Training component");}
		if (IsPrefabRef(training.gameObject)){
			GameObject o = CreatePrefab(training.gameObject, new Vector3(0,99999f,0f), Quaternion.identity);
			training = o.GetComponent<Training>();
		}
		if (training.sample == null){
			training.Compile();
		}
		if (output == null){
			Transform ot = transform.Find("output-overlap");
			if (ot != null){output = ot.gameObject;}}
		if (output == null){
			output = new GameObject("output-overlap");
			output.transform.parent = transform;
			output.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }
		for (int i = 0; i < output.transform.childCount; i++){
			GameObject go = output.transform.GetChild(i).gameObject;
			if (Application.isPlaying){Destroy(go);} else {DestroyImmediate(go);}
		}
		group = new GameObject(training.gameObject.name).transform;
		group.parent = output.transform;
		group.SetPositionAndRotation(output.transform.position, output.transform.rotation);
        group.localScale = new Vector3(1f, 1f, 1f);
        rendering = new GameObject[width, depth];
		model = new OverlappingModel(training.sample, N, width, depth, periodicInput, periodicOutput, symmetry, foundation, predetermined);
        undrawn = true;
    }

    public void RunPreplacing()
    {	
		preplacement.Run(this);
    }

    void OnDrawGizmos(){
		Gizmos.color = Color.cyan;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(width*gridsize/2f-gridsize*0.5f, depth*gridsize/2f-gridsize*0.5f, 0f),
							new Vector3(width*gridsize, depth*gridsize, gridsize));
	}

	public void Run(){
		if (model == null){return;}
        if (undrawn == false) { return; }
        if (model.Run(seed, iterations)){
			Draw();
            if (!undrawn) 
                generationDone.Invoke();
        }
        else
        {
			Debug.Log("Generation failed");
			Generate();
        }
	}

	public GameObject GetTile(int x, int y){
		return rendering[x,y];
	}

	public void Draw(){
		if (output == null){return;}
		if (group == null){return;}
        undrawn = false;
		try
        {
            for (int y = 0; y < depth; y++){
				for (int x = 0; x < width; x++){
					if (rendering[x,y] == null){
						if (IsRemoved(y * width + x))
							continue;

                        int v = (int)model.Sample(x, y);
						if (v != 99 && v < training.tiles.Length){
							Vector3 pos = new Vector3(x*gridsize, y*gridsize, 0f);
							int rot = (int)training.RS[v];
							GameObject fab = training.tiles[v] as GameObject;
							if (fab != null){
								GameObject tile = (GameObject)Instantiate(fab, new Vector3() , Quaternion.identity);
								Vector3 fscale = tile.transform.localScale;
								tile.transform.parent = group;
								tile.transform.localPosition = pos;
								tile.transform.localEulerAngles = new Vector3(0, 0, 360 - (rot * 90));
								tile.transform.localScale = fscale;
								rendering[x,y] = tile;
                            }
						} else
                        {
                            undrawn = true;
                        }
					}
				}
            }
        } catch (IndexOutOfRangeException) {
	  		model = null;
	  		return;
	  	}
        if (undrawn)
            return;
        AutoTile();
		if (postprocessing)
		    postprocessing.Run(this);
	}

    private void AutoTile()
    {
        if (!autoTile) return;
        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (rendering[x, y] == null) continue;
                var at = rendering[x, y].GetComponent<Autotilling>();
                if (at != null)
                    at.AutoTile(rendering, x, y);
            }
        }
    }

    public override void Upscale(int scale)
    {
        int width = rendering.GetLength(0);
        int height = rendering.GetLength(1);

        GameObject[,] upscaled = new GameObject[width * scale, height * scale];

        var upGroup = new GameObject(training.gameObject.name).transform;
        upGroup.parent = output.transform;
		upGroup.SetPositionAndRotation(output.transform.position, output.transform.rotation);

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                for (int i = 0; i < scale; i++)
                {
                    for (int j = 0; j < scale; j++)
                    {
                        var o = rendering[w, h] != null ? 
                            Instantiate(rendering[w, h], new Vector3(w * scale + i, h * scale + j, 0), Quaternion.identity) 
                            : null;
                        upscaled[w * scale + i, h * scale + j] = o;
                        o.transform.parent = upGroup;
                    }
                }
            }
        }
        rendering = upscaled;
        Clear();
        group = upGroup;

        generationDone.Invoke();
    }

    public override void AddBlocked(int index)
    {
		removed.Add(index);
    //    predetermined.Add(new Predetermined(index, new byte[]{1,1,1,1,1,1,1,1,1}, true));
    }

    private bool[,] GetPreplacement()
    {
        var ret = new bool[width, depth];
        for (int i = 0; i < ret.GetLength(1); i++)
            for (int j = 0; j < ret.GetLength(0); j++)
                ret[j, i] = true;

        return ret;
    }
}

 #if UNITY_EDITOR
[CustomEditor (typeof(OverlapWFC))]
public class WFCGeneratorEditor : Editor
{
    private OverlapWFC generator;
    private bool showWfc;
    private bool advanced;

    private void OnEnable()
    {
        generator = (OverlapWFC) target;
    }

	public override void OnInspectorGUI () {
        if (generator.training != null){
			if(GUILayout.Button("generate")){
				generator.Generate();
			}
			if (generator.model != null){
				if(GUILayout.Button("RUN")){
					generator.Run();
				}
			}
		}

        generator.training = (Training)EditorGUILayout.ObjectField(
            "Training", generator.training, typeof(Training), true);
        generator.preplacement = (Preplacement)EditorGUILayout.ObjectField(
            "Tile preplacement", generator.preplacement, typeof(Preplacement), false);
		generator.postprocessing = (WfcPostprocessing)EditorGUILayout.ObjectField(
            "Postprocessing", generator.postprocessing, typeof(WfcPostprocessing), false);

		showWfc = EditorGUILayout.BeginFoldoutHeaderGroup(showWfc, "WFC DATA");
        if (showWfc)
        {
            generator.N = EditorGUILayout.IntField("N", generator.N);
            generator.width = EditorGUILayout.IntField("Width", generator.width);
            generator.depth = EditorGUILayout.IntField("Height", generator.depth);
            generator.symmetry = EditorGUILayout.IntField("Symmetry", generator.symmetry);
            generator.iterations = EditorGUILayout.IntField("Iterations", generator.iterations);
		}
        EditorGUILayout.EndFoldoutHeaderGroup();

        advanced = EditorGUILayout.BeginFoldoutHeaderGroup(advanced, "Advanced");
        if (advanced)
        {
            generator.debug = EditorGUILayout.Toggle("Debug", generator.debug);
            generator.seed = EditorGUILayout.IntField("Seed", generator.seed);
			generator.periodicInput = EditorGUILayout.Toggle("Periodic Input", generator.periodicInput);
            generator.periodicOutput = EditorGUILayout.Toggle("Periodic Output", generator.periodicOutput);
            generator.autoTile = EditorGUILayout.Toggle("AutoTile", generator.autoTile);
        }

		EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
#endif