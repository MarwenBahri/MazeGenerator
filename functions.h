#ifndef FUNCTIONS_H_INCLUDED
#define FUNCTIONS_H_INCLUDED
#include <stdbool.h>
typedef struct cellule
{
    int x;
    int y;
    struct cellule* right;
    struct cellule* left;
}cellule;
typedef struct GetRandom //liste cha�n�e qui sert � g�n�rer un entier al�atoire qui n'�tait choisit pr�c�demment
{
    int val;
    struct GetRandom* next;

}GetRandom;
typedef struct GetRandom* GR_List;

void generate_grille(cellule* G[],int nx,int ny);//remplire G par nx*ny addresses de cellules
void generate_lab(cellule* G[],int nx,int ny);//cr�e le labyrinthe

GR_List create_link(cellule* T[],cellule* G[],int nx,int ny,int* lengthT,int index_ofcc,int i_target,int dir_x,int dir_y,int c_x,int c_y,GR_List GR,bool switched);// cr�e le lien entre G[i_target] et G[index_ofcc]
int get_closest_cell(cellule* T[],cellule* G[],int nx,int* lengthT,int r);//retourne la cellule de T la plus proche de G
int get_random(GetRandom* GR,int n);//retourne un entier al�atoire choisi � partir de GR
GR_List update_list(GetRandom* GR,int r,int n);//r�tirer la cellule correspondante � n, c�d qui pour laquelle val==n
GR_List create_list(GetRandom* GR,int n);//cr�e la liste chain�e qui sert � g�n�rer des entiers al�atoires distinctes
void show_lab(cellule* G[],int nx,int ny);//affiche le labyrinthe
#endif // FUNCTIONS_H_INCLUDED
