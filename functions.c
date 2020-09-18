#include <stdlib.h>
#include <stdio.h>
#include <stddef.h>
#include <time.h>
#include <stdbool.h>
#include "functions.h"

void generate_grille(cellule* G[],int nx,int ny)
{

    int i = 0;//compteur sur G de 0 à nx*ny-1
    for(int y=0;y<ny;y++)
    {
        for(int x=0;x<nx;x++)
        {
            cellule* p = malloc(sizeof(cellule));//allocation de mémoire pour une cellule
            while(!p)p=malloc(sizeof(cellule));//test sur l'allocation
            p->x = x;
            p->y = y;
            p->right = NULL;
            p->left = NULL;
            G[i] = p;
            i++;
        }
    }

}
/*---------------------------------------------*/
GR_List create_list(GR_List GR,int n)
{
    //crée la liste chainée qui va être utilisée pour générer un entier aléatoire entre 0 et n-1
    int i = 1;
    GetRandom* first = malloc(sizeof(GetRandom));
    first->val = 0;
    first->next = NULL;
    GR = first;
    GetRandom* p = first;//
    while(i<n)
    {
        GetRandom* m = malloc(sizeof(GetRandom));
        while(!m)m=malloc(sizeof(GetRandom));
        m->val =  i;
        m->next = NULL;

        p->next = m;
        p = m;
        i++;

    }
    return GR;

}
/*---------------------------------------------*/
GR_List update_list(GR_List GR,int val_toremove,int n)
{
    //cette fonction permet de retirer l'entier (ou l'indice) le plus récemment choisi
    if(GR->val == val_toremove)
    {
        GR_List ptr = GR->next;
        free(GR);
        return ptr;
    }else
    {
        GetRandom* previous = GR;
        GetRandom* ptr = GR->next;

        while(ptr->val!= val_toremove && ptr->next!=NULL)
        {
            previous = ptr;
            ptr = previous->next;
        }
        if(ptr->next==NULL)
        {
            previous->next = NULL;
            free(ptr);
        }else
        {
            previous->next = ptr->next;
            free(ptr);
        }
        return GR;
    }

}
/*---------------------------------------------*/
int get_random(GR_List GR,int n)
{
    //cette fonction retourne un entier aléatoire entre 0 et n-1 tout en assurant qu'il n'était pas choisi précédemment
    srand(time(NULL));
    int r = rand()%n;
    int i = 0 ;
    GetRandom* h_pointer = GR;
    while(i<r)
    {
        h_pointer = h_pointer->next;
        i++;
    }
    int val_toremove = h_pointer->val;

    return val_toremove;
}
/*---------------------------------------------*/
int get_closest_cell(cellule* T[],cellule* G[],int nx,int* lengthT,int r)
{

    int index = 0;
    int i = 1;
    int px = abs(T[0]->x - G[r]->x);
    int py = abs(T[0]->y - G[r]->y);
    int s = px+py;
    while(i<(*lengthT))
    {

        px = abs(T[i]->x - G[r]->x);
        py = abs(T[i]->y - G[r]->y);
        if(s>(py+px))
        {
            s = py+px;
            index = i;
        }
        i++;
    }
    int G_index = T[index]->x + (T[index]->y * nx);
    return G_index;
}
/*---------------------------------------------*/
GR_List create_link(cellule* T[],cellule* G[],int nx,int ny,int* lengthT,int index_ofcc,int i_target,int dir_x,int dir_y,int c_x,int c_y,GR_List GR,bool switched)
{

    if(G[index_ofcc]!=G[i_target])
    {

        if((G[index_ofcc]->left!=NULL)&&(G[index_ofcc]->right!=NULL))
        {
            T[*lengthT] = G[i_target];
            *lengthT = *lengthT + 1;

            return create_link(T,G,nx,ny,lengthT,i_target,index_ofcc,-dir_x,-dir_y,c_x,c_y,GR,true);//just inverse the direction
        }else
        {


            srand(time(NULL));

            int h_or_v = rand()%2;
            int next_i = index_ofcc + h_or_v*dir_x*c_x*c_y +(1-c_y)*dir_x + (1-(h_or_v*c_x))*dir_y*nx*c_y; // formule qui détermine l'indice dans G de la prochaine cellule

            /*srand(time(NULL));
            cette partie du code permet de choisir aléatoirement quel pointeur parmi left et right à remplir
            bool goleft = false;
            bool goright = false;
            if (G[index_ofcc]->left == NULL && G[index_ofcc]->right != NULL)goleft = true;
            if (G[index_ofcc]->left != NULL && G[index_ofcc]->right == NULL)goright = true;
            int l_or_r = rand()%2;//left or right
            if((l_or_r == 0 && !goright ) || goleft) //l_or_r == 0 => we go left
            {
                G[index_ofcc]->left = G[next_i];
            }else
            {
                G[index_ofcc]->right = G[next_i];
            }*/
            if(G[index_ofcc]->left == NULL && G[index_ofcc]->right != NULL)
            {
                G[index_ofcc]->left = G[next_i];

            }else if (G[index_ofcc]->left != NULL && G[index_ofcc]->right == NULL)
            {
                G[index_ofcc]->right = G[next_i];
            }else
            {
                G[index_ofcc]->left = G[next_i];
            }

            if(next_i != i_target)
            {
                //showGR(GR);
                T[*lengthT] = G[next_i];
                *lengthT = *lengthT + 1;
                GR = update_list(GR,next_i,nx*ny-(*lengthT));
                //showGR(GR);

                if(G[next_i]->x == G[i_target]->x)c_x = 0;
                if(G[next_i]->y == G[i_target]->y)c_y = 0;
                return create_link(T,G,nx,ny,lengthT,next_i,i_target,dir_x,dir_y,c_x,c_y,GR,switched);
            }else if(!switched)
            {
                T[*lengthT] = G[next_i];
                *lengthT = *lengthT + 1;
                return GR;
            }else
            {
                return GR;
            }

        }

    }else{
        return GR;
    }
}
/*---------------------------------------------*/
void generate_lab(cellule* G[],int nx,int ny)
{
    double time_spent = 0.0;
    clock_t begin = clock();

    cellule* T[nx*ny];
    srand(time(NULL));
    int r = rand()%(nx*ny);//générer un entier aléatoire entre 0 et nx*ny-1
    T[0] = G[r];//ajouter G[r] à T

    int lengthT = 1;

    GR_List GR = NULL;//GetRandom*
    GR = create_list(GR,nx*ny);//liste chainée qui va assurer qu'à chaque fois on tombe sur un entier aléatoire qui n'etait pas encore utilisé;

    int n = nx*ny;//la taille de la liste chainée
    GR = update_list(GR,r,n);//retirer r de la liste chainée pour qu'il ne soit pas choisi une autre fois

    int dir_x = 1;// 1 si on doit aller dans le sens des x croissants -1 si non
    int dir_y = 1;//de même
    int c_x = 1;// possibilité de changer la valeur de x, si = 1 on peut changer si = 0 on ne peut pas
    //c'est-à-dire que la cellule de départ et la cellule cible sont stuées dans la même colonne (ont la même valeur de x)
    int c_y = 1;// de même
    //show_lab(G,nx,ny);
    while(lengthT<n)
    {
        int i_target = get_random(GR,n-(lengthT)); //la cellule de G à ajouter à T
        GR = update_list(GR,i_target,n);


        int index_ofcc = get_closest_cell(T,G,nx,&lengthT,i_target);//l'indice dans G de la plus proche cellule de T à G[i_target]

        if((G[index_ofcc]->x - G[i_target]->x)>0)dir_x = -1; //on doit se déplacer dans le sens des x décroissantes
        if((G[index_ofcc]->y - G[i_target]->y)>0)dir_y = -1; //de même
        if(G[i_target]->x == G[index_ofcc]->x)c_x = 0;//on doit pas changer la coulonne
        if(G[i_target]->y == G[index_ofcc]->y)c_y = 0;//on doit pas changer le ligne
        GR = create_link(T,G,nx,ny,&lengthT,index_ofcc,i_target,dir_x,dir_y,c_x,c_y,GR,false);//créer le lien entre G[index_ofcc] et G[i_target] et mettre T à jour

        dir_x = 1;
        dir_y = 1;
        c_x = 1 ;
        c_y = 1 ;

        //show_lab(G,nx,ny);


    }

    clock_t end = clock();
    time_spent = (double)(end-begin)/CLOCKS_PER_SEC;
    printf("\n Time taken to create the maze was: %f seconds\n",time_spent);

    time_spent = 0.0;
    begin = clock();
    show_lab(G,nx,ny);//cette foncton sert à afficher le labyrinthe en utilisant le tableau de pointeurs G
    end = clock();
    time_spent = (double)(end-begin)/CLOCKS_PER_SEC;
    printf("\n Time taken to print the maze was: %f\n",time_spent);
}
/*---------------------------------------------*/
void show_lab(cellule* G[],int nx,int ny)
{
    printf("\n");
    printf("\n");
    int n = nx*ny;
    int i = 0;//compteur sur G
    printf(".");
    for(int s=0;s<nx;s++)printf("__.");
    printf("\n");
    for(int y = 0;y<ny;y++)
    {
        printf("|");
        for(int x = 0;x<nx;x++)
        {
            if(i+nx < n)
            {
                if(!(G[i]->left == G[i+nx] || G[i]->right == G[i+nx] || G[i+nx]->left == G[i] || G[i+nx]->right == G[i]))
                {
                    printf("__");
                }else
                {
                    printf("..");
                }
            }else
            {
                printf("__");
            }

            if(i+1<n)
            {
               if(!(G[i]->left == G[i+1] || G[i]->right == G[i+1] || G[i+1]->left == G[i] || G[i+1]->right == G[i]))
                {
                    printf("|");
                }else
                {
                    printf(":");
                }
            }else
            {
                printf("|");
            }
            i++;

        }
        printf("\n");
    }
     printf("\n");
      printf("\n");
}
