﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpillMotion : MonoBehaviour {

    public List<Resource> activeResources;

    [SerializeField]
    private Spawner[] spillSpawners;
    [SerializeField]
    private SpriteRenderer turnbull;
    [SerializeField]
    private Image spillMotionImage;

    private bool firstWarning;
    private bool warningActivated;
    private GameController game;
    private Player player;
    private bool entered = false;
    private bool exited = false;
    private float opacity = 1f;
    private Color color;

    // Use this for initialization
    void Start() {
        firstWarning = true;
        warningActivated = false;
        game = GetComponent<GameController>();
        player = FindObjectOfType<Player>();
    }

    void OnEnable() {
        if (!player)
            player = FindObjectOfType<Player>();
        if (!game)
            game = FindObjectOfType<GameController>();
        GetActiveResources(false);

    }

    void OnDisable() {
        entered = false;
        exited = false;
        opacity = 1f;
        game.gameSpeed = 1f;
        warningActivated = false;
    }

    void Update() {
        if (warningActivated) {
            if (StartSpill()) {

                if (player.popularity == 0.0f) {
                    SceneManager.LoadScene("Loss");
                } else if (player.popularity > 50) {
                    if (activeResources.Count == 0)
                        GetActiveResources(true);
                    if (EndSpill()) {
                        game.Continue();
                        enabled = false;
                    }
                }
                player.popularity += Time.deltaTime;
            }
        } else
            DisplayWarning();
    }

    void DisplayWarning() {
        if (!firstWarning) {
            warningActivated = true;
            return;
        }
        if (!spillMotionImage.gameObject.activeSelf) {
            spillMotionImage.gameObject.SetActive(true);
        } else {
            IncreaseWarningOpacity();
            if (opacity == 1f) {
                if (Input.anyKeyDown || Input.touchCount > 0) {
                    ClearWarning();
                    spillMotionImage.gameObject.SetActive(false);
                    warningActivated = true;
                }
            }
        }
    }

    bool StartSpill() {
        if (!entered) {
            FadeTurnbull(true);
            ReduceResourceOpacity();
            if (opacity == 0.0f) {
                entered = true;
                ClearResources();
                opacity = 1;
                game.gameSpeed = 1f;
            }
            return false;
        }
        player.SetState(Player.State.Spill, 0, 0, true);
        ToggleSpawners(true);
        game.gamePaused = false;
        return true;
    }

    bool EndSpill() {
        if (!exited) {
            FadeTurnbull(false);
            game.gamePaused = true;
            ReduceResourceOpacity();
            if (opacity == 0.0f) {
                exited = true;
                ClearResources();
                opacity = 1;
            }
            return false;
        }
        player.SetState(Player.State.Default, 0, 0, false);
        ToggleSpawners(false);
        GetActiveResources(true);
        game.gamePaused = false;
        return true;
    }

    void FadeTurnbull(bool _input) {
        color = turnbull.color;
        color.a = (_input) ? Mathf.Clamp01(color.a + Time.deltaTime) : Mathf.Clamp01(color.a - Time.deltaTime);
        turnbull.color = color;
    }

    public void GetActiveResources(bool input) {
        activeResources = FindObjectsOfType<Resource>().Where(n => n.spill == input).ToList();
    }

    public void ClearResources() {
        if (activeResources.Count > 0) {
            foreach (Resource r in activeResources) {
                Destroy(r.gameObject);
            }
            activeResources.Clear();
        }
    }

    private void ToggleSpawners(bool _input) {
        foreach (Spawner spawner in spillSpawners) {
            spawner.gameObject.SetActive(_input);
        }
    }

    private void ReduceResourceOpacity() {
        opacity = Mathf.Clamp01(opacity - Time.deltaTime);
        foreach (Resource r in activeResources) {
            color = r.GetComponent<SpriteRenderer>().color;
            color.a = opacity;
            r.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void IncreaseWarningOpacity() {
        opacity = Mathf.Clamp01(opacity + (Time.deltaTime * 2));
        color = spillMotionImage.color;
        color.a = opacity;
        spillMotionImage.color = color;
    }
    private void ClearWarning() {
        color = spillMotionImage.color;
        color.a = 0;
        spillMotionImage.color = color;
    }
}