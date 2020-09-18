#include <stdlib.h>
#include <stdio.h>
#include "functions.h"

void main()
{
    int nx,ny;
    printf("donner nx: ");
    scanf("%d",&nx);
    printf("donner ny: ");
    scanf("%d",&ny);

    cellule* G[nx*ny];//initialiser le tableau de pointeurs G de taille nx*ny
    generate_grille(G,nx,ny);//remplir G avec nx*ny addresses de cellules

    generate_lab(G,nx,ny);//construire le labyrinthe
}
