
// ---- LISTE ------


(*

Una lista e' una sequenza finita di elementi dello stesso tipo.
Il tipo di una lista e' definito mediante il costruttore  list (*type constructor*).


DEFINIZIONE DEL TIPO LIST
^^^^^^^^^^^^^^^^^^^^^^^^^

Sia T un tipo qualunque (anche polimorfo). L'espressione
   
      T list 
  
denota il tipo di una lista i cui elementi hanno tipo T

========    

Esempi:

             int list  --->  lista di interi 

            char list  --->  lista di caratteri  

  (int * string) list  --->  lista i cui elementi sono coppie int * string
   
    (int -> int) list  --->  lista i cui elementi sono funzioni di tipo int -> int

      (int list) list  --->  lista i cui elementi sono liste di interi
      
             'a  list  --->  lista i cui elementi hanno tipo 'a

 ('a * int * 'a)  list --->  lista i cui elementi hanno tipo 'a * int * 'a
             

Notare che:

-  Il costruttore list ha precedenza piu' alta di * e ->.

   Quindi:

      int * string list    equivale a      int * (string list)  
                                           // coppie della forma (n : int, ls : string list ) 
                                    
                                              
      int -> float list    equivale a        int -> (float list)
                                             // funzioni da int a float list

- list e' associativo a sinistra

   Quindi

     int list list     equivale a     (int list) list
                                      // liste di liste di int   
                                          

Una lista i cui elementi hanno tipo T e' rappresentata da un termine F# di tipo 'T list'.


DEFINIZIONE TERMINI DI TIPO T LIST
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Sia T un tipo qualunqe (anche polimorfo).
I termini di tipo 'T list' sono definiti induttivamente come segue:

(1) [** BASE INDUZIONE **]

   [] e' un termine di tipo 'T list'.
   []  rappresenta la lista vuota.

(2) [** PASSO INDUTTIVO **]

    Supponiamo che xs sia un termine di tipo 'T list' e x sia un termine di tipo T. Allora 
   
       x :: xs

    e' un termine di  tipo 'T list'.
    Inoltre, se xs rappresenta la lista contenente gli elementi

      x1 ; ...  ;  xn
      
    il  termine  'x :: xs' rappresenta la lista contenente gli elementi 

            x ; x1 ; ...  ;  xn

    ossia la lista ottenuta  ponendo x in testa alla lista xs.

    Nel termine  x :: xs:

    -   ::  e'  un operatore chiamato  *cons*.
    -   x   e' detta la  *testa (head)*  della lista
   -   xs   e' detta la   *coda (tail)*  della lista.

   Il termine x :: xs puo' essere rappresentato dall'albero
   
                       :: (cons)
                     /    \             x : T           
                    /      \           xs : T list
                   x       xs  
              (testa)       (coda) 


L'operatore :: (cons) associa a destra:

      x1  ::  x2 :: xs    equivale a       x1  ::  (x2 :: xs) 
        



NOTA
^^^^

Dalla definizione, segue che un termine ha tipo 'T list'  SE E SOLO SE
e' costruibile partendo dalla lista vuota
applicando un numero finito di volte il passo induttivo
(ogni volta cons va applicato a termini di tipo T e 'T list').


Esempi di liste:
^^^^^^^^^^^^^^^^

i)    1 :: []     

        ::
       /  \
      1     []

Termine di tipo 'int list' avente testa 1 e coda []      
Rappresenta la lista contente l'elemento 1


ii)    2 ::  1 :: []   

        ::
       /  \
      2    ::
          /  \
         1    []


Lista di tipo 'int list' avente testa 2 e per coda la lista '2::[]'.
Rappresenta la lista contenente gli elementi 2 e 1.


iii)  "due" ::  1 :: []


Questo termine non e' ben tipabile.
Infatti, una espressione della forma

   "due" :: xs

ha senso solo se xs ha tipo 'string list'.
Nel nostro caso, a destra di cons c'e' il termine
   
        1 :: []

che ha tipo 'int list'.

    

SINTASSI ALTERNATIVA
^^^^^^^^^^^^^^^^^^^^

Per rappresentare una lista, anziche' usare l'operatore cons si possono
scrivere gli elementi in essa contenuti fra parentesi quadre, separati da ;

Ad esempio:

     [1]       equivale a      1 :: []
 [2 ; 1]       equivale a      2 :: 1 :: []  

L'uso di cons e'  utile nel pattern matching.


EAGER EVALUATION
^^^^^^^^^^^^^^^^

Una espressione di tipo 'T list' e' valutata in modo *eager*.
Questo significa che, prima di costruire la lista, vengono valutati i suoi elementi.

Esempio:


let ls = [ 10 + 5 ; 12 - 2 ; 3 * 4 ] ;;

L'interprete crea il seguente binding

 val ls : int list = [15; 10; 12]

in cui nella lista compaiono i valori delle espressioni usate nella definizione.

Verificare cosa produce la valutazione di 

let ls = [ 10 + 5 ; 12 / 0 ; 3 * 4 ] ;;


TIPO LISTA VUOTA
^^^^^^^^^^^^^^^^

Il termine [] rappresenta una lista vuota di un qualunque tipo T.
Il tipo di [] e' quindi:

   [] : 'a list    // termine polimorfo



NOTA SULLE ESPRESSIONI POLIMORFE 
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

============================================================================================
|  At top-level, polymorphic expressions are allowed only if they are value expressions.   |
|  Polymorphic expressions can be used freely for intermediate results.                    |
===========================================================================================
(HR, pag. 81)

Le motivazioni e il significato della regola sara' chiarito piu' avanti.

Per ora ci interessa notare che applicazioni di funzioni della forma

   f  a  

eseguite  a top-level (cioe', non all'interno di una espressione piu' complessa)
e che hanno tipo polimorfo  non sono ammesse.


Ad esempio, consideriamo la funzione polimorfa 

   List.rev : 'a list -> 'a list

che inverte una lista.

Esempio

 List.rev [ 1 ; 2 ; 3 ]

produce la lista  [ 3 ; 2 ; 1 ] 
   

L'applicazione

  List.rev [] ;;

non e' ammessa in quanto ha tipo polimorfo  'a list.
Per poterla eseguire,  occorre annotare l'argomento []
in modo che l'espressione abbia un tipo concreto (non polimorfo).

Esempi di applicazioni corrette:


List.rev ( [] : int list );;

List.rev ( [] : (char * int) list list );;




*)


// Esempi  di liste

[] ;;    // lista vuota
// val it : 'a list = []

1 :: [] ;;
// val it : int list = [1]

let l1 = 100 :: 1 :: [] ;;
//  ìval l1 : int list = [100; 1]

(*

l1 e' la lista

                ::
               /   \
              /     \
            100      ::
                     / \
                    /   \
                   1     []


*) 



let l2 = 200 :: l1  ;;
//  l2 e' la lista [200 ; 100 ; 1 ] di tipo int list


let l3 =  [200 ; 100 ; 1 ] ;;
// l3 = l2


let bl = [ true ; false ; true ] ;;
//val bl : bool list = [true; false; true]


let bl1 =  [ 1 < 2;  1 = 2 ;  true ] ;;
// val bl1 : bool list = [true; false; true]
// Notare la eager evaluation (gli elementi sono valutati prima di costruire la lista)

let fl  = [ 3.1 ; 2.6 ; 7.8 ; 1.56 ] ;;
// val fl : float list = [3.1; 2.6; 7.8; 1.56]
 
let pl = [ ("a",3) ; ("ggg",6) ] ;;
// val pl : (string * int) list = [("a", 3); ("ggg", 6)]
 

let funl = [ cos ; sin ;  fun x -> x * 3.5 ]  ;;
// val funl : (float -> float) list  - lista di funzioni float -> float

 
let ll= [ [1;2] ; [3;4] ] ;;
//val ll : int list list - lista di liste di interi
 
// Attenzione a non scrivere , al posto di ; (ricordare che , costruisce una tupla)

let x1l = [ "uno" ; "due" ] ;;
let x2l = [ "uno" , "due" ] ;;
// che differenza c'e' fra xl1 e xl2 ?

(**

ESERCIZIO
^^^^^^^^^

Scrivere qual e' il tipo e il valore dei seguenti termini; verificare la risposta usando l'interprete.


  [ 100 / 2 ; 100 / 2]             [ 100 / 2 , 100 / 2 ]
  int list                          (int * int) list

  [ "100 / 2"  ;  "100 / 2 " ]    [  100 / 2  ;  "100 / 2 " ]  [  100 / 2  ,  "100 / 2 " ]
  string list                     errore, non tipato            (int*string) list
  [ 100 / 2 ; 100 / 1 ; 100 / 0]   
  errore, divido per zero

    
**)


//   --------  RANGE EXPRESSION  ---------

(*  Le range espression sono espressioni che permettono di generare liste *)   

// Esempi (vedere i dettagli sul libro)

[3 .. 10] ;;   // lista degli interi fra 3 e 10
// val it : int list = [3; 4; 5; 6; 7; 8; 9; 10]

[3 .. 2 .. 20] ;;   // lista degli interi fra 3 e 10 con passo 2
// val it : int list = [3; 5; 7; 9; 11; 13; 15; 17; 19]

[10 .. -1 .. 0] ;;   // passo negativo
// val it : int list = [10; 9; 8; 7; 6; 5; 4; 3; 2; 1; 0]

['c' .. 'f'];;  // lista dei caratteri fra 'c' e 'f'
// val it : char list = ['c'; 'd'; 'e'; 'f']


//   -------  PATTERN SU LISTE  ----------

(* Per definire pattern su liste e' utile usare l'operatore cons *)

 
let y :: ys = ['a' .. 'f' ] ;;
//  val y : char = 'a'
//  val ys : char list = ['b'; 'c'; 'd'; 'e'; 'f']
(*  y e' legato alla testa della lista, ys alla coda *)

let x0 :: x1 :: xs = [0 .. 10 ] ;;

(*

val x0 : int = 0    // primo elemento
val x1 : int = 1    // secondo elemento 
val xs : int list = [2; 3; 4; 5; 6; 7; 8; 9; 10]  // resto della lista

*)



// ---  RICORSIONE SU LISTE ---


(*

La forma generale di una funzione ricorsiva che ha argomento
composto da una lista ls e':

let rec f ... ls ... =
 match ls with 
 | [] -> v
 | x::xs -> .... f xs ...

Vengono distinti due casi:

- CASO BASE:

  Se ls e' la lista vuota, la funzione f restituisce il valore v

- CASO INDUTTIVO:

  Supponiamo che ls abbia la forma x :: xs
  Per calcolare il valore da restituire, si chiama ricorsivamente f  sulla lista xs

Il caso induttivo e' ben fondato in quanto la chiamata ricorsiva e' fatta
sulla lista xs che e' piu' piccola della lista ls di partenza.

Come si vede negli esempi e negli esercizi, lo schema sopra puo' richiedere
ulteriori raffinamenti (piu' casi da considerare). Occorre prestare attenzione
che le chiamate ricorsive coinvolgano liste piu' piccole di quella di partenza.

Va inoltre evitato di esplodere inutilmente il pattern matching,
considerando  'casi particolari' che non sono tali.


*)


(*

Esempio 1
^^^^^^^^^

Definire la funzione ricorsiva

   sumlist : int list -> int 

che calcola la somma degli elementi di una lista.

====

Supponiamo che ls sia la lista

   ls = [ x0 ; x1 ; x2 ; ... ; xn ]

La ricorsione si basa sul fatto che

    x0 + x1 + x2 + .... + xn  =  x0  +  (x1 + x2 + .... + xn)

Per calcolare

   x1 + x2 + .... + xn

posso chiamare ricorsivamente sumlist sulla sottolista

  [ x1 ; x2 ; ... ; xn ]  // coda di ls


Quindi, data una lista ls  avente testa x0 e coda xs, vale

  
   sumlist  ls =  x + sumlist xs      (#)

La ricorsione e' usata in modo fondato perche' la lista xs e' piu' piccola di ls.


Il caso base e' la definizione di


   sumlist []

Che valore deve avere 'sumlist []' ?

Supponiamo che ls contenga solo l'elemento x

   ls = [x]

ls ha testa  x e coda [].
La definizione induttiba (#), ponendo ls = [x] si riscrive come:

   sumlist [x] = x  + sumlist []

D'altra parte, sappiamo che  'sumlist [x]' deve valere x.
Segue che

    sumlist [] = 0

In altri termini, 'sumlist []' e' l'elemento neutro della somma.    


*)  


let rec sumlist ls =  
    match ls with 
        | [] -> 0    
        | x0::xs -> x0 + sumlist xs ;; 
 
let sum1 = sumlist [1 .. 10] ;;  // 55


 
(*

Esempio 2
^^^^^^^^^

Definire la funzione ricorsiva

  sumprod : int list -> int * int
  
che data una list ls restituisce la coppia (sum,prod),
dove sum e' la somma degli elementi della lista
e prod e' il prodotto degli elementi della lista.
Nella definizione di sumprod non va usata la funzione sumlist definita oggi.


Attenzione a definire correttamente il valore di

  sumprod []

(vedere la discussione all'esempio precedente)

*)  

let rec sumprod ls =  
    match ls with 
        | [] -> (0,1) 
        | x0::xs ->  
            let (sum,prod) = sumprod xs 
            ( x0 + sum, x0 * prod) ;;


let sp = sumprod [1..10] ;; // (55, 3628800)

 


(*

Esempio 3
^^^^^^^^^


Definire la funzione ricorsiva (polimorfa)

     len : 'a list -> int

che calcola la lunghezza di una lista

NOTA
^^^^^

In F# e' definita l'analoga funzione List.length


*)
 
 
let rec len ls =  
    match ls with 
    | [] -> 0 
    | x::xs -> 1 + len xs 
 
len [3..7] ;;   // 5
len ['a'..'z'] ;;  // 26

(*

Notare che anche la soluzione

let rec len ls =  
    match ls with 
    | [] -> 0
    | [x] -> 1
    | x::xs -> 1 + len xs 

e' corretta.

Tuttavia non c'e' nessun motivo per trattare a parte il caso [x] (lista con un solo elemento).
Soluzioni di questo tipo, in cui si aggiungono casi particolari inutili,  vanno evitate

*)   



 
(*


Esempio 4
^^^^^^^^^

Definire la funzione ricorsiva (polimorfa)

     append : 'a list * 'a list -> 'a list

che concatena due liste. Quindi

   
       append(l1, l2) = lista contenente gli elementi di l1
                        seguiti dagli elementi di l2
                          
NOTA
^^^^^
La funzione append e' gia' implementata in F# dall'operatore @,
che e' un operatore infisso con associativita' a destra.

Esempio

 [1 ; 2 ] @ [ 3 ; 4 ] =  [ 1 ; 2 ; 3 ; 4 ]

 [1 ; 2 ] @ [ 3 ; 4 ] @ [5]  =  [ 1 ; 2 ; 3 ; 4 ; 5 ]


*) 
 
let rec append (xs, ys) = 
    match xs with 
    | [] -> ys 
    | z::zs -> z :: append (zs, ys) 
 
 
append ( [ 1 ; 2] , [ 3 ; 4 ; 5 ] ) ;; //  [1; 2; 3; 4; 5]

(*

NOTA SU  ::  e   @
^^^^^^^^^^^^^^^^^^

Attenzione a usare correttamente gli operatori :: (cons) e @ (append)

  ::     ha  operandi di tipo T e 'T list'
   @     ha entrambi gli operandi 'T list'.

Per inserire un elemento x : T  in testa a una lista xs : T list si puo' scrivere

   x  :: xs    OPPURE     [x] @ xs

E' preferibile usare  cons (piu' efficiente).

Per inserire un  elemento x : T  in coda a una lista xs : T list si puo' solo usare  @

   xs @  [x]

mentre   xs :: x   non ha senso (errore di tipo).


*)   



(*


Esempio 5
^^^^^^^^^

Definire la funzione ricorsiva (polimorfa)

   rev : 'a list -> 'a list

che inverte gli elementi di una lista (analoga a List.rev):

  rev [ x0 ; x1 ; .... ; x(n-1) ; xn ]  = [ xn ; x(n-1) ; ... ; x1 ; x0 ]  

===

Notare che il pattern matching permette di estrarre il primo elemento della lista
ma non l'ultimo.

La lista 
 
     [ xn ; x(n-1) ; ... ; x1 ; x0 ]  

puo' pero' essere vista come la concatenazione delle due liste

   [ xn ; x(n-1) ; ... ; x1 ]      [x0]

Inoltre [ xn ; x(n-1) ; ... ; x1 ] puo' essere costruita con la chiamata ricorsiva

  rev [ x1 ; .... ; x(n-1) ; xn ]   // l'argomento di rev e' la coda di ls
  


*)
 
let rec rev ls =  
    match ls with 
    | [] -> [] 
    | x :: xs -> append (rev xs, [x]) 
 

rev [1 .. 10] ;;        // [10; 9; 8; 7; 6; 5; 4; 3; 2; 1]

(*

Esempio 6
^^^^^^^^^

Definire la funzione ricorsiva (polimorfa)

   mem : 'a * 'a list -> bool   when 'a : equality

che verifica l'appartenenza di un elemento in una lista

Anche in questo caso e' gia' definita in F# l'analoga funzione List.mem


===

Si osserva che

    x  appartiene a  [ y0 ; y1 ; .... ;  yn ] 

se e solo se

 ( x = y0 )    OR   ( x appartiene a  [ y1 ; .... ;  yn ] )


*)   
 
let rec mem (x , ls) =  
    match ls with 
    | [] -> false 
    | y::ys -> x=y || mem (x, ys) 
     
mem ('b', ['a' .. 'h'] ) ;; // true
mem ('x', ['a' .. 'h'] ) ;; // false
 
 
