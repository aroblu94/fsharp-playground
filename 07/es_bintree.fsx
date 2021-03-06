#r @"FsCheck";;
open FsCheck;;

type 'a binTree =
    | Null    // empty tree
    | Node of 'a  * 'a binTree * 'a binTree ;;   // Node(root, left, right)

let t2 = Node (2, Null, Node ( 4 , Null, Null ) );; 
let t7 = Node (7, Null, Node (10, Null, Node ( 13 , Null, Null ))) ;; 
let t8 = Node ( 8, Node ( 11, Null, Null), Null ) ;; 
let t5 = Node ( 5, t7, t8 ) ;;
let t9 = Node ( 9, Null,   Node (12, Null, Null) );
let t6 = Node( 6, t9, Null) ;;
let t3 = Node(3, t5, t6 ) ;;
let t1 = Node (1, t2, t3 );; 

(*
    TRASFORMAZIONE DI ALBERI

    Definire la funzione ricorsiva

       intToFloatTree : int binTree -> float binTree

    che trasforma un albero di int in un albero di float.
    Piu' precisamente, applicando la funzione a un albero di interi,
    si ottiene un albero con gli stessi valori, i cui nodi pero' hanno tipo float.
*)
let rec intToFloatTree btree = 
    match btree with
    | Null -> Null
    | Node(x,left,rigth) -> 
        let r = float x // r = radice nuovo albero
        let left1 = intToFloatTree left // left1 = albero di float corrispondente a left
        let rigth1 = intToFloatTree rigth // rigth1 = albero di float corrispondente a rigth
        // ora che ho costruito tutti pezzi del mio nuovo albero
        // posso ricostruire l'albero di float da restituire:
        Node(r,left1,rigth1);;

(*
    Avrei potuto anche direttamente costruire un nuovo albero, senza prima crearne i vari pezzetti prima:
    Node(r, intToFloatTree left, intToFloat rigth)
*)

(*
    VISITA DI UN ALBERO

    Definire la funzione ricorsiva

      inorderToList : 'a binTree -> 'a list
     
    che attraversa un albero in inorder e restituisce la lista dei nodi dell'albero
    nell'ordine in cui sono stati visitati.

    visita inorder sottoalbero sin, radice,  visita inorder sottoalbero dx
*)
let rec inorderToList btree =
    match btree with
    | Null ->  []
    | Node(x,l,r) ->
        inorderToList l @ [x] @ inorderToList r ;;

(*
    Preorder
    preorderToList

    radice, left, rigth
*)
let rec preorderToList btree =
    match btree with
    | Null ->  []
    | Node(x,l,r) ->
        [x] @ preorderToList l @ preorderToList r ;;


// definizione prop_visit
let prop_visit (btree : int binTree) =
  let l1 = inorderToList btree  |> List.sort  // visita inorder e ordina i valori 
  let l2 = preorderToList btree |> List.sort
  l1 = l2 ;;
// prop_visit :  E.binTree -> bool

(*
    RICERCA DI UN ELEMENTO

    Definire la funzione ricorsiva

        search : 'a * 'a binTree -> bool     when 'a : equality
       
    che controlla l'appartenenza di un nodo a un albero.
*)
let rec search (a, btree) =
    match btree with
    | Null -> false
    | Node(x,l,r) when x=a -> true
    | Node(x,l,r) -> search(a,l) || search(a,r);;

let rec searchInList (a, l) =
    match l with
    | [] -> false
    | x::xs when x=a -> true
    | x::xs -> searchInList (a,xs) ;; 

// controllo con quick check
let prop_search ( x : int , btree : int binTree) =
    let l = inorderToList btree
    searchInList(x,l) = search(x,btree)
;;

Check.Quick prop_search;;

(*
    RICERCA CON PREDICATO FILTRO

    i) Definire la funzione ricorsiva

       filterToList : ('a -> bool) * 'a binTree -> 'a list

    che, data una coppia formata da un predicato pred e un albero binario btree,
    cioe' una coppia  della forma

       ( pred : 'a -> bool , btree :  'a binTree )


    visita l'albero btree in inorder e costruisce la lista degli elementi visitati 
    che soddisfano il predicato pred.
    In altri termini, pred funge da filtro che scarta gli elementi x per cui 'pred x' non vale.

    ii) Definire i predicati:

    isEven   :  int -> bool       //  isEven  n  = true   sse  n e' un numero pari  
    isSmall  :  int -> bool       //  isSmall n  = true   sse  n  < 5
*)

let isEven x =
    match x%2 with
    | 0 -> true
    | _ -> false ;;

let isSmall x =
    match x with
    | _ when x<5 -> true
    | _ -> false ;;

let rec filterToList (fn: ('a -> bool), btree) =
    match btree with
    | Null ->  []
    | Node(x,l,r) ->
        match fn(x) with
        | true -> filterToList (fn, l) @ [x] @ filterToList (fn, r)
        | false -> filterToList (fn, l) @ filterToList (fn, r) ;;


let t1even = filterToList (isEven,t1 ) ;;
// val t1even : int list = [2; 4; 10; 8; 12; 6]

let t1small = filterToList (isSmall,t1 ) ;;
// val t1small : int list =   [2; 4; 1; 3]

(*
    CONTEGGIO DI NODI E FOGLIE
    ==========================

    i) Definire la funzione ricorsiva

      count : 'a binTree -> int * int

    che dato un albero binario btree calcola la coppia (nodes, leaves) dove:
    - nodes   e' il numero totale dei nodi dell'albero;
    - leaves  e' il numero totale delle foglie dell'albero.
*)
let rec count btree =
    match btree with
    | Null -> (0,0)
    | Node(x,Null,Null) -> (1,1)
    | Node(x,l,r) -> 
        let (nln,nll) = count l
        let (nrn,nrl) = count r
        (1+nln+nrn,nll+nrl) ;;

(*
    PROFONDITA' DI UN NODO
    =======================

    Definire la funzione ricorsiva

     depthToList : int * 'a binTree -> 'a list

    che data la coppia 

      ( n : int  , btree : 'a binTree )   con n>=0, 

    restituisce la lista dei nodi di btree aventi profondita' n.
*)
let rec depthToList (n, btree) =
    match btree with
    | Null -> []
    | Node(x,Null,Null) when n>0 -> []
    | Node(x,l,r) ->
        if n=0 then [x]
        else depthToList(n-1, l) @ depthToList(n-1, r);;


(*
    PERCORSO IN UN ALBERO
    =====================

    Per accedere a un elemento di un albero,  partiamo dalla radice
    e indichiamo il percorso da compiere.
    Un percorso e' una lista di direzioni da seguire, destra (R) o sinistra (L).
    Se la lista e' vuota, il percorso termina immediatamente nella radice,
    altrimenti si seguono le indicazioni del percorso, scendendo a ogni passo
    nel figlio sinistro o destro a seconda della direzione.
    E' possibile che il percorso non possa essere completato
    (ad esempio, se in un nodo senza figlio sinistro la prossima direzione e' L).

    Per denotare una direzione introduciamo il tipo

    type direction = L | R ;; // Left, Right

    Definire la funzione ricorsiva

     getElement : direction list * 'a binTree -> 'a option

    che data la coppia

      ( path : direction list , btree : 'a binTree )

    effettua il percorso path nell'albero btree e restituisce:

    - Some k   se il percorso termina sul nodo k
    -  None    se non e' possibile completare il percorso.
*)
type direction = L | R ;; // Left, Right

let rec getElement (path, btree) =
    match btree with
    | Null -> "None"
    | Node(x,l,r) ->
        match path with
        | [] -> "Some " + x.ToString()
        | x::xs ->
            if x=L then getElement(xs,l)
            else getElement(xs,r) ;;





(*
=================================================================================

*** ESERCIZI PARTE 2 (ALBERI BINARI DI RICERCA)  ***
*)
(*
    INSERIMENTO DI UN ELEMENTO
    ==========================

    i) Definire la funzione ricorsiva

       insert :  'a  * 'a binTree -> 'a binTree    when 'a : comparison
                                                   

    che data la coppia   

     ( x : 'a  , btree : 'a binTree ) 

    dove btree e' un albero binario di ricerca,
    restituisce l'albero binario di ricerca ottenuto inserendo x in btree;
    se x e' gia' nell'albero, va restituito l'albero di partenza
    (non sono ammessi nodi duplicati).
*)
(*let rec insert (x, btree) =
    match btree with
    | Null -> Node(x, Null, Null)
    | Node(r, Null, Null) -> 
        if x<r then Node(r, Node(x,Null,Null), Null)
        elif x>r then Node(r, Null, Node(x,Null,Null))
        else Node(r, Null, Null)
    | Node(r,left,right) ->
        if x<r then insert(x, left)
        else insert(x, right) ;;*)
let rec insert (x,btree) =
    match btree with
    | Null -> Node(x,Null,Null)
    | Node(r,left,right) when x<r -> Node(r, insert(x,left), right)
    | Node(r,left,right) when x>r -> Node(r, left, insert(x,right)) 
    | Node(_,_,_) as n -> n;;

// es
let t9 = Node(8, Node(6,Null,Node(7,Null,Null)),Null);;
insert(5,t9);; // Node(8, Node(6,Node(5,Null,Null),Node(7,Null,Null)), Null)
insert(6,t9);;
insert(10,t9);;

(*
    ii) Definire la funzione ricorsiva 

       insertFromList : 'a list * 'a binTree -> 'a binTree    when 'a : comparison

    che, data la coppia  

       ( ls: 'a list , btree : 'a binTree ) 

    dove btree e' un albero binario di ricerca, restituisce l'albero binario di ricerca 
    ottenuto inserendo gli elementi della lista ls nell'albero btree.
    Gli elementi vanno inseriti nell'ordine in cui compaiono nella lista.
    (notare che, cambiando l'ordine con cui gli elementi sono inseriti, 
    l'albero di ricerca ottenuto puo' risultare diverso).
*)
let rec insertFromList (list, btree) =
    match list with
    | [] -> btree
    | [x] -> insert(x,btree)
    | x::xs -> insertFromList(xs, insert(x,btree)) ;;

// es
insertFromList([5;6;10],t9);; 
// -> Node (8,Node (6,Node (5,Null,Null),Node (7,Null,Null)),Node (10,Null,Null))

(*
    iii) Definire le seguenti liste:

    let intList = [ 20 ; 10 ; 60 ; 15 ; 40 ; 100 ; 30 ; 50 ; 70 ; 35 ; 42 ; 58 ; 75 ; 32 ; 37 ] ;;
    let strList1 = [ "pesca" ; "banana" ; "uva" ; "albicocca" ; "nocciola" ; "ribes" ] ;;
    let strList2 = [ "limone" ; "ciliegia" ; "mela" ; "pera" ; "noce"  ] ;;
     
    Costruire i seguenti alberi di ricerca:

    - intTree : albero ottenuto partendo dall'albero vuoto e  inserendo gli elementi di intList 
                (albero di tipo 'int binTree')

    - strTree1:  albero ottenuto partendo dall'albero vuoto e inserendo gli elementi di strList1 
                 (albero di tipo 'string binTree')

    - strTree2: albero ottenuto inserendo in strTree1 gli elementi di strList2 
                (albero di tipo 'string binTree')
*)
let intList = [ 20 ; 10 ; 60 ; 15 ; 40 ; 100 ; 30 ; 50 ; 70 ; 35 ; 42 ; 58 ; 75 ; 32 ; 37 ] ;;
let strList1 = [ "pesca" ; "banana" ; "uva" ; "albicocca" ; "nocciola" ; "ribes" ] ;;
let strList2 = [ "limone" ; "ciliegia" ; "mela" ; "pera" ; "noce"  ] ;;

let intTree = insertFromList(intList,Null) ;;
let strTree1 = insertFromList(strList1,Null) ;;
let strTree2 = insertFromList(strList2,strTree1) ;;

(*  
    RICERCA DI UN ELEMENTO
    ======================

    Definire la funzione ricorsiva

     search1 : 'a * 'a binTree -> bool   when 'a : comparison

    definita come search (vedi esercizio sopra), 
    ma in cui si assume che l'albero sia un albero binario di ricerca 
    (quindi, la ricerca puo' essere effettuata in maniera efficiente).

    Verificarne la correttezza facendo dei test sugli alberi gia' definiti.
*)

let rec search1 (a,btree) =
    match btree with
    | Null -> false
    | Node(x,l,r) when a=x -> true
    | Node(x,l,r) -> 
        if a<x then search(a,l)
        else  search(a,r) ;;

// test
search1(100,intTree);; // true
search1("ciliegia",strTree2);; //true
search("mango", strTree1);; //false

(*
    RICERCA MINIMO ELEMENTO
    =======================

    Definire la funzione ricorsiva

       min : 'a binTree -> 'a option

    che, dato un albero binario di ricerca btree, restituisce il minimo elemento dell'albero;
    piu' precisamente,  se btree e' vuoto, la funzione  restituisce None;
    altrimenti la funzione restituisce 'Some m', dove m e' il minimo elemento di btree.

    Notare che in un albero di ricerca il minimo elemento si trova scendendo verso sinistra 
    fin che si puo'.
*)
let rec min btree =
    match btree with
    | Null -> "None"
    | Node(x,Null,_) -> "Some " + x.ToString()
    | Node(x,l,_) -> min l ;;

// test
min intTree ;;   //   Some 10
min strTree2;;   //  Some "albicocca" =====> PROBLEMA CON IL TIPO (sprintf "%i" richiede un int)
min ( Null : int binTree) ;; // None

(*
    ESTRAZIONE SOTTOALBERO
    ======================

    Definire la funzione ricorsiva

      subtree : 'a * 'a binTree -> 'a binTree when 'a : comparison

    che, data la coppia  

       ( x: 'a , btree : 'a binTree ) 

    con btree albero binario di ricerca,
    restituisce il sottoalbero di btree con radice x. 
    Se x non compare in tree, va restituito l'abero vuoto.
*)
let rec subtree (x,btree) =
    match btree with
    | Null -> Null
    | Node(v,Null,Null) -> 
        if x=v then Node(v,Null,Null)
        else Null
    | Node(v,l,r) ->
        if x<v then subtree(x,l)
        elif x>v then subtree(x,r) 
        else Node(v,l,r);;

let m1 = min ( subtree(10, intTree) )  ;;    // Some 10
let m2 = min ( subtree(15, intTree) )  ;;    // Some 15
let m3 = min ( subtree(60, intTree) )  ;;    // Some 30
let m4 = min ( subtree(40, intTree) ) ;;     // Some 30
let m5 = min ( subtree(100, intTree) ) ;;    // Some 70
let m6 = min ( subtree(1000, intTree) ) ;;   // None
// ------
let m7 = min ( ( subtree ("limone",  strTree2) ) ) ;;  //  Some "ciliegia"
let m8 = min ( ( subtree ("ribes",  strTree2) ) )  ;;  // Some "ribes"


(*
    RICERCA DI UN ELEMENTO CON CALCOLO PERCORSO SEGUITO
    ===================================================

    [Difficile]  Definire la funzione ricorsiva
      
      searchPath : 'a * 'a binTree -> 'a list when 'a : comparison

    che  data la coppia  

        ( x: 'a , btree : 'a binTree ) 

    con btree albero binario di ricerca,
    restituisce la lista dei nodi corrispondenti al percorso seguito per raggiungere
    il nodo x partendo dalla radice dell'albero. 
    Se x non e' nell'albero, va restituita la lista vuota.

    Evitare di attraversare l'albero piu' di una volta.
*)
let rec searchPath (x,btree) =
    match btree with
    | Null -> []
    | Node(v,_,_) when v=x -> [v]
    | Node(v,l,r) when search1(x,btree) ->
        let path = [v]
        if x<v then path @ searchPath(x,l)
        else path @ searchPath(x,r)
    | Node(v,l,r) -> [] ;;

// es
let p1 = searchPath (10, intTree) ;; // [20; 10]
let p2 = searchPath (20, intTree) ;; // [20]
let p3 = searchPath (40, intTree) ;; // [20; 60; 40]
let p4 = searchPath (32, intTree) ;; // [20; 60; 40; 30; 35; 32]
let p5 = searchPath (11, intTree) ;; // []