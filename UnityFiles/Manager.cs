using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
  

public class Manager : MonoBehaviour
{
    
    unsafe struct cellule
    {
        public int x;
        public int y;
        public cellule* left;
        public cellule* right;
    };
    
    unsafe cellule[] generate_grille( int nx, int ny)
    {

        cellule[] G = new cellule[nx * ny];
        int i = 0;
        for (int y = 0; y < ny; y++)
        {
            for (int x = 0; x < nx; x++)
            {
                
                G[i] = new cellule();
                G[i].x = x;
                G[i].y = y;
                G[i].left = null;
                G[i].right = null;

                i++;
            }
        }

        return G;
    }

    unsafe int get_closest_cell(cellule[] T, cellule[] G, int nx,  int lengthT, int r)
    {

        int index = 0;
        int i = 1;
        int px = Math.Abs((T[0].x - G[r].x));
        int py = Math.Abs((T[0].y - G[r].y));
        int s = px + py;
        while (i < (lengthT))
        {
            px = Math.Abs((T[i].x - G[r].x));
            py = Math.Abs((T[i].y - G[r].y));
            if (s > (py + px))
            {
                s = py + px;
                index = i;
            }
            i++;
        }
        
        int G_index = T[index].x + (T[index].y * nx);
        return G_index;
    }
    unsafe void create_link(cellule[] T, cellule[] G, int nx, int ny, ref int lengthT, int index_ofcc, int i_target, int dir_x, int dir_y, int go_x, int go_y,List<int> GR,bool switched)
    {
 
            if ((G[index_ofcc].left != null) && (G[index_ofcc].right != null))
            {
                T[lengthT] = G[i_target];
                lengthT++;
                create_link(T, G, nx, ny, ref lengthT, i_target, index_ofcc, -dir_x, -dir_y, go_x, go_y, GR, true) ;//just inverse the direction
            }
            else
            {
                
                bool goleft = false;
                bool goright = false;
                if (G[index_ofcc].left == null && G[index_ofcc].right != null) goleft = true;
                if (G[index_ofcc].left != null && G[index_ofcc].right == null) goright = true;
                System.Random rnd = new System.Random();
                int h_or_v = rnd.Next(0, 2);
                int next_i = index_ofcc + h_or_v * dir_x * go_x * go_y + (1 - go_y) * dir_x + (1 - (h_or_v * go_x)) * dir_y * nx * go_y;
            
                int l_or_r = rnd.Next(0, 2);//left or right
                if ((l_or_r == 0 && !goright) || goleft)
                {
                    fixed (cellule* p = &G[next_i])
                    {
                        G[index_ofcc].left = p;
                    }               
                }
                else
                {
                    fixed (cellule* p = &G[next_i])
                    { 
                        G[index_ofcc].right = p;
                    }
                }
                
                
                if (next_i != i_target)
                {
                    GR.Remove(next_i);
                    if (G[next_i].x == G[i_target].x) go_x = 0;
                    if (G[next_i].y == G[i_target].y) go_y = 0;
                    T[lengthT] = G[next_i];
                    lengthT++;
                    create_link(T, G, nx, ny, ref lengthT, next_i, i_target, dir_x, dir_y, go_x, go_y, GR,switched);
                }else if (!switched)
                {
                    T[lengthT] = G[next_i];
                    lengthT++;
                }

            }
        
    } 
    unsafe void generate_lab(cellule[] G, int nx, int ny)
    {

        cellule[] T = new cellule[nx*ny];
        System.Random rnd = new System.Random();
        int r = rnd.Next(0,nx * ny);

        T[0] = G[r];
        int lengthT = 1;
        int n = nx * ny;
        
        List<int> GR = new List<int>();
        for (int i = 0; i < n; i++) GR.Add(i);
        GR.Remove(r);      

        int dir_x = 1;
        int dir_y = 1;
        int go_x = 1;
        int go_y = 1;
        
        while (lengthT < n)
        {            
            int i_target = rnd.Next(0, n - lengthT);
            
            i_target = GR[i_target];
            GR.Remove(i_target);
            int index_ofcc = get_closest_cell(T, G, nx,  lengthT, i_target);
            if ((G[index_ofcc].x - G[i_target].x) > 0) dir_x = -1; 
            if ((G[index_ofcc].y - G[i_target].y) > 0) dir_y = -1; 
            if (G[i_target].x == G[index_ofcc].x) go_x = 0;
            if (G[i_target].y == G[index_ofcc].y) go_y = 0;
            create_link(T, G, nx, ny, ref lengthT, index_ofcc, i_target, dir_x, dir_y, go_x, go_y, GR,false);                                                                                                     
            dir_x = 1;
            dir_y = 1;
            go_x = 1;
            go_y = 1;       
        }       
    }

    public GameObject Wall;
    public Text Xfield;
    public Text Yfield;
    
    
    public Transform maze;
    public Transform upperWall;
    public Transform lowerWall;
    public Transform leftWall;
    public Transform rightWall;

    public GameObject clearbutton;
    public Button generatebutton;

    public NavMeshSurface NavSurface;
    public Transform MazeSurface;


    public GameObject PlayerPrefab;

    Vector3 startPosition;
    Vector3 endPosition;
    unsafe public void generate()
    {
        generatebutton.interactable = false;
        
        int nx = int.Parse(Xfield.text);
        int ny = int.Parse(Yfield.text);
        MazeSurface.localScale = new Vector3(nx, 0.2f, ny);

        MazeSurface.position = new Vector3((float)(nx % 2) / 2, MazeSurface.position.y, (float)((ny + 1)%2)/2);
        cellule[] G = new cellule[nx * ny];
        G = generate_grille(nx, ny);
        
        generate_lab(G, nx, ny);
        Vector3 SpawnPosition = new Vector3(-nx/2, 0.5f,ny/2);
        Vector3 nextPosition;
        int n = nx * ny;
        GameObject tempWall;

        #region GenerateWalls
        int k = 0;
        for(int l = 0; l < nx; l++)
        {
            nextPosition = new Vector3(SpawnPosition.x + l+ 0.5f, 0.5f, SpawnPosition.z  + 0.5f);
            tempWall = Instantiate(Wall, nextPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
            tempWall.transform.parent = upperWall;
        }
        for (int j=0;j<ny;j++)
        {
            nextPosition = new Vector3(SpawnPosition.x , 0.5f, SpawnPosition.z - j);
            Instantiate(Wall, nextPosition, Quaternion.identity , leftWall);
            
            for (int i=0;i<nx;i++)
            {
                if (k + nx < n)
                {
                    fixed(cellule* c1 = &G[k])
                    {
                        fixed(cellule* c2 = &G[k+nx])
                        {
                            if (!(c1->left == c2 || c1->right == c2 || c2->left == c1 || c2->right == c1))
                            {
                                nextPosition = new Vector3(SpawnPosition.x + i  + 0.5f, 0.5f, SpawnPosition.z - j  - 0.5f);       
                                tempWall = Instantiate(Wall, nextPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                                tempWall.transform.parent = maze;
                            }
                        }
                    }
                    

                }
                else
                {

                    nextPosition = new Vector3(SpawnPosition.x + i  + 0.5f, 0.5f, SpawnPosition.z - j - 0.5f);
                    tempWall = Instantiate(Wall, nextPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                    tempWall.transform.parent = lowerWall;
                }
                if (k + 1 < n)
                {

                    fixed (cellule* c1 = &G[k])
                    {
                        fixed (cellule* c2 = &G[k + 1])
                        {
                            if (!(c1->left == c2 || c1->right == c2 || c2->left == c1 || c2->right == c1))
                            {

                                nextPosition = new Vector3(SpawnPosition.x + i  +1.0f , 0.5f, SpawnPosition.z - j);
                                if((k+1)%nx==0)
                                {
                                    Instantiate(Wall, nextPosition, Quaternion.identity, rightWall);
                                }
                                else
                                {
                                    Instantiate(Wall, nextPosition, Quaternion.identity, maze);
                                }
                                
                            }
                        }
                    }
             
                }
                else
                {
                    nextPosition = new Vector3(SpawnPosition.x + i + 1.0f, 0.5f, SpawnPosition.z - j);
                    Instantiate(Wall, nextPosition, Quaternion.identity,rightWall);

                }
                k++;

            }
        }
        #endregion
        #region Enter/Exit
        List<int> ids = new List<int>();
        for (int i = 0; i < 4; i++) ids.Add(i);
        System.Random rnd = new System.Random();
        int r = rnd.Next(0, 4);
        startPosition = removewall(ids[r],nx,ny);

        ids.Remove(ids[r]);
        r = rnd.Next(0, 3);
        endPosition = removewall(ids[r],nx,ny);
        #endregion
        NavSurface.BuildNavMesh();
        SpawnButton.interactable = true;
    }
    public Vector3 removewall(int id,int nx ,int ny)
    {
        
        System.Random rnd = new System.Random();
        int t;
        switch (id)
        {
            case 0:
                
                t= rnd.Next(0, nx); 
                upperWall.GetChild(t).gameObject.SetActive(false);
                return upperWall.GetChild(t).transform.position;
            case 1:
                
                t = rnd.Next(0, nx);
                lowerWall.GetChild(t).gameObject.SetActive(false);
                return lowerWall.GetChild(t).transform.position;
            case 2:
                t = rnd.Next(0, ny);
                rightWall.GetChild(t).gameObject.SetActive(false);
                return rightWall.GetChild(t).transform.position;
            case 3:
                t = rnd.Next(0, ny);
                leftWall.GetChild(t).gameObject.SetActive(false);
                return leftWall.GetChild(t).transform.position;
            default:
                return Vector3.zero;
        }
        
    }
    public Text followtext1;
    
    public void clearwalls()
    {
        #region 
        foreach (Transform child in maze)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in upperWall)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lowerWall)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in leftWall)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in rightWall)
        {
            Destroy(child.gameObject);
        }
        #endregion

        generatebutton.interactable = true;
        GoButton.GetComponent<Button>().interactable = true;
        GoButton.SetActive(false);
        SpawnButton.interactable = false;
        PlayerPrefab.SetActive(false);
        FollowButton.interactable = false ;
        followtext1.text = "Follow";
        followtext1.color = new Color(255f, 255f, 255f, 10f);


        CharacterMovement.won = false;
        if (CameraMovement.startfollowing || followed)
        {
            CameraMovement.startfollowing = false;
            Vector3 oldrot = GetComponent<CameraMovement>().oldrotation;
            //Vector3 oldpos = GetComponent<CameraMovement>().oldpos;
            CameraMovement.unfollow = true;
           // Vector3 smoothed = Vector3.Lerp(transform.position, oldpos, 0.125f);
           // Camera.main.transform.position = smoothed;
            Camera.main.transform.rotation = Quaternion.Euler(oldrot);
            followed = false;


        }
        


    }
    public static bool followed = false;
    #region sliders
    public void slide_x(float value)
    {
        Vector3 pos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(value, pos.y, pos.z);
    }
    public void slide_z(float value)
    {
        Vector3 pos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(pos.x , pos.y, value);
    }
    public void slide_y(float value)
    {
        Vector3 pos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(pos.x, value, pos.z);
    }
    #endregion




    public Button SpawnButton;
    public GameObject GoButton;
    public Button  FollowButton;
    public void SpawnCharacter()
    {

        //Vector3 spawnpos = new Vector3(startPosition.x, 0.3f, startPosition.z);
        //Myplayer = Instantiate(PlayerPrefab, spawnpos, Quaternion.identity) as GameObject;
        //PlayerControls.endPosition = endPosition;
        //PlayerControls.winPosition = endPosition;
        CharacterMovement.winPosition = endPosition;
        PlayerPrefab.transform.position = startPosition;
        PlayerPrefab.SetActive(true);
        SpawnButton.interactable = false;
        GoButton.SetActive(true);
        FollowButton.interactable = true;
        followtext1.color = new Color(255f, 255f, 255f, 255f);


    }
    public void Gooo()
    {



        PlayerPrefab.GetComponent<CharacterMovement>().goonow();
        GoButton.SetActive(false);
        //Myplayer.GetComponent<PlayerControls>().Agent.SetDestination(endPosition);
        //Myplayer.GetComponent<PlayerControls>().Player.Move(Myplayer.GetComponent<PlayerControls>().Agent.desiredVelocity, false, false);
        //PlayerControls.startmoving = true;
        // Myplayer.GetComponent<Animator>().speed = 1;
        //Myplayer.GetComponent<Animator>().SetBool("Win", false);
        //Myplayer.GetComponent<Animator>().SetBool("Run", true);
        //GoButton.SetActive(false);
        //PlayerControls.n = 1;
        //PlayerControls.win = true;


    }
    
}

