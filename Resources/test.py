def alg(zad, czas = 120):
    suma = sum(zad)
    obl = lambda x, s: x.pro()*czas/suma
    k = []
    for z in zad:
        k.append("{0:8} - czas {1}".format(z.name, obl(z, suma)))
    return k

class Zadanie:
    def __init__(self, prio, naz):
        self.name = naz
        self.priority = prio
        self.progress = 0.

    def pro(self):
        return self.priority*(1-self.progress)

    def __add__(self, other):
        return self.pro() + other.pro()

    def __radd__(self, other):
        return self.pro() + other

def dump(l):
    for e in l:
        print(e)
    
zadania = [Zadanie(1, "matma"), Zadanie(1, "tv"), Zadanie(1, "poczta")]
lsa = alg(zadania)
c1 = 120
print("{}min".format(c1))
dump(lsa)
print("-----------poczta w polowei")
c2 = c1 -20
print("{}min".format(c2))
zadania[-1].progress = 0.5 #postep poczta = 50%
lsb = alg(zadania, c2)
dump(lsb)
print("-----------gotowa tv")
#print("10 minut niczego")
c3 = c2 -40# -10
print("{}min".format(c3))
del zadania[1] #usun tv
lsc = alg(zadania, c3)
dump(lsc)