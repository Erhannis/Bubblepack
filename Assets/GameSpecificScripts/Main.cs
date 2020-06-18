﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Esprima.Ast;

public class Main : MonoBehaviour {
    private enum GameState {
        NORMAL, DEAD, WON
    }

    private System.Random rand = new System.Random();

    private int TARGET_FPS = 60;

    private const int PX_PER_UNIT = 100;
    private const float BOARD_WIDTH = 22.0f; //TODO Calc from screen?
    private const float TIMESCALE = 2f; //TODO Time

    private Color BG_COLOR = Color.black;

    private Rect playBounds;
    private List<Circle> circles;
    private GameState state;
    private float requiredDensity;

    void Start() {
        //        int startingCircles = (int)(float)SceneChanger.globals["starting_circles_float"];
        //        float requiredDensity = (float)SceneChanger.globals["required_density_float"];
        //        Init(startingCircles, requiredDensity);
        Init(2, 0.6f);
    }

    void Init(int startingCircles, float requiredDensity) {
        this.requiredDensity = requiredDensity;
        state = GameState.NORMAL;
        playBounds = new Rect(-BOARD_WIDTH/2, -BOARD_WIDTH/2, BOARD_WIDTH, BOARD_WIDTH);
        circles = new List<Circle>();
        for (int i = 0; i < startingCircles; i++) {
            float r = UnityEngine.Random.Range(0.1f, 4f);
            var c = new Circle(new Vector2(UnityEngine.Random.Range(-BOARD_WIDTH/2+r, BOARD_WIDTH/2-r), UnityEngine.Random.Range(-BOARD_WIDTH/2+r, BOARD_WIDTH/2-r)), genPastel());
            c.radius = r;
            Utils.logOnceE("//TODO Check/filter initial circles");
            circles.Add(c);
        }
    }

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }

    private Color genPastel() {
        List<int> rgbHat = new List<int>{ 0, 1, 2 };
        float[] rgb = { 0, 0, 0 };
        int a = rgbHat[rand.Next(0, rgbHat.Count)];
        rgb[a] = 1f;
        rgbHat.Remove(a);
        int b = rgbHat[rand.Next(0, rgbHat.Count)];
        rgb[b] = UnityEngine.Random.Range(0f, 1f);
        rgbHat.Remove(b);
        int c = rgbHat[rand.Next(0, rgbHat.Count)];
        rgb[c] = UnityEngine.Random.Range(rgb[b], rgb[a]);
        rgbHat.Remove(c);
        return new Color(rgb[0],rgb[1],rgb[2],1f);
    }

    public Text text_u;
    private long count_u = 0;
    private double ms_u = 0;
    private System.Diagnostics.Stopwatch sw_u = new System.Diagnostics.Stopwatch();
    // Update is called once per frame
    void Update() {
        sw_u.Restart();
        sw_u.Start();

        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;
        if (Screen.height > Screen.width) {
            Camera.main.GetComponent<Camera>().orthographicSize = 1.1f * (0.5f * playBounds.width * Screen.height) / Screen.width;
        } else {
            Camera.main.GetComponent<Camera>().orthographicSize = 1.1f * (0.5f * playBounds.width * Screen.width) / Screen.width;
        }

        Camera.main.backgroundColor = BG_COLOR; //TODO Move elsewhere?


        //Debug.Log("//TODO Remove reset key");
        if (Input.GetKeyDown("r")) {
            Init((int)(float)SceneChanger.globals["starting_circles_float"], (float)SceneChanger.globals["required_density_float"]);
            return;
        }
        // if (Input.GetKeyDown("q")) {
        //     SceneChanger.staticLoadScene("MainMenu");
        //     return;
        // }


        // if (Input.GetKeyDown(""+(i+1))) {
        //     turn(dials[i], !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
        //     checkWin();
        // }
        // if (Input.GetKeyDown("s")) {
        //     scramble();
        //     checkWin();
        // }

        //doPlayerInput(); // No player input in autonomous mode

        checkWin();

        sw_u.Stop();
        //TimeSpan ts = sw_u.Elapsed;
        //ms_u += ts.TotalMilliseconds;
        //count_u++;
        //if (count_u >= 10) {
        //    text_u.text = "" + (ms_u / count_u);
        //    count_u = 0;
        //    ms_u = 0;
        //}
    }

    private static Color DEAD_BG = new Color(0.5f, 0f, 0f);
    private static Color WON_BG = new Color(0f, 0.5f, 0f);
    private void checkWin() {
        if (state != GameState.NORMAL) {
            return; // Shrug
        }
        float filledArea = 0;
        Utils.logOnceE("//TODO Check hit play bounds");
        foreach (var a in circles) {
            filledArea += a.area();
            // Check touching bounds
            if (a.pos.x - a.radius <= playBounds.x || a.pos.y - a.radius <= playBounds.y || a.pos.x + a.radius >= playBounds.x + playBounds.width || a.pos.y + a.radius >= playBounds.y + playBounds.height) {
                state = GameState.DEAD;
                a.color = Color.red;
                //TODO Highlight play bounds?
                Camera.main.backgroundColor = DEAD_BG;
                return;
            }
            // Check touching circles
            foreach (var b in circles) {
                if (a != b) {
                    if (a.touching(b)) {
                        // Dead
                        state = GameState.DEAD;
                        a.color = Color.red;
                        b.color = Color.red;
                        Camera.main.backgroundColor = DEAD_BG;
                        return;
                    }
                }
            }
        }
        float totalArea = playBounds.width * playBounds.height;
        if (totalArea != 0) {
            if (filledArea / totalArea >= requiredDensity) {
                state = GameState.WON;
                Camera.main.backgroundColor = WON_BG;
                return;
            }
        }
    }

    private void checkInputDir(Vector3 cursorPos) { // Scrap
        var ray = Camera.main.ScreenPointToRay(cursorPos);
        Vector2 tap = new Vector2(ray.origin.x, ray.origin.y).normalized;
        RaycastHit hit;
        Physics.Raycast(ray.origin, Vector3.forward, out hit);
        if (hit.transform == null) {
            // Didn't hit any of the UI button colliders
        } else {
            return;
        }
    }

    private void doPlayerInput() {
        if (Input.GetKeyDown("d")) {
        }


        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                //MPos3 idir = checkInputDir(touch.position);
            }
        }

        //if (!foundMov && Input.GetMouseButtonDown(0) && !(uiUpBtnDown || uiDownBtnDown)) { // Left click (0-left,1-right,2-middle)
        //    MPos3 idir = checkInputDir(Input.mousePosition);
        //    if (idir != null) {
        //        dir += idir;
        //        foundMov = true;
        //    }
        //}
    }

    static Material lineMaterial;
    static void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public Text text_oro;
    private long count_oro = 0;
    private double ms_oro = 0;
    private System.Diagnostics.Stopwatch sw_oro = new System.Diagnostics.Stopwatch();

    // Will be called after all regular rendering is done
    public void OnRenderObject() {
        sw_oro.Restart();
        sw_oro.Start();

        var vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        var horizExtent = vertExtent * Screen.width / Screen.height;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        foreach (var c in circles) {
            drawCircle(c.pos, c.radius, true, c.color);
        }

        // Play area
        GL.Begin(GL.LINE_STRIP);
        GL.Color(Color.white);
        GL.Vertex3(playBounds.xMin, playBounds.yMin, -1f);
        GL.Vertex3(playBounds.xMax, playBounds.yMin, -1f);
        GL.Vertex3(playBounds.xMax, playBounds.yMax, -1f);
        GL.Vertex3(playBounds.xMin, playBounds.yMax, -1f);
        GL.Vertex3(playBounds.xMin, playBounds.yMin, -1f);
        GL.End();

        GL.PopMatrix();

        sw_oro.Stop();
        //TimeSpan ts = sw_oro.Elapsed;
        //ms_oro += ts.TotalMilliseconds;
        //count_oro++;
        //if (count_oro >= 10) {
        //    text_oro.text = "" + (((float)ms_oro) / count_oro);
        //    ms_oro = 0;
        //    count_oro = 0;
        //}
    }

    private int lineCount = 100;

    private void drawCircle(Vector3 pos, float r, bool filled, Color color) {
        if (filled) {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i) {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(pos.x, pos.y, pos.z);
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        } else {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i) {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        }

    }
}
