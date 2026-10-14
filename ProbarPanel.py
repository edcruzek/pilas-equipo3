"""Pruebas reproducibles de la API. Ejecutar con PanelPilas iniciado."""
import json,sys,urllib.request,urllib.error
base=sys.argv[1] if len(sys.argv)>1 else 'http://127.0.0.1:5222'
checks=0
def check(value,label):
 global checks
 assert value,label
 checks+=1
def post(path,payload):
 request=urllib.request.Request(base+path,data=json.dumps(payload).encode(),headers={'Content-Type':'application/json'})
 try:
  with urllib.request.urlopen(request,timeout=10) as r:return r.status,json.load(r)
 except urllib.error.HTTPError as e:return e.code,json.load(e)
for representation in ['arreglo','enlazada']:
 values=[]
 for value in ['10','20','30']:
  status,r=post('/api/pila',dict(tipo=representation,operacion='push',elementos=values,dato=value))
  check(status==200,'push');values=r['elementos'];check(values[0]==value,'tope')
 status,r=post('/api/pila',dict(tipo=representation,operacion='peek',elementos=values))
 check(r['elementos']==values and '30' in r['mensaje'],'peek no destructivo')
 status,r=post('/api/pila',dict(tipo=representation,operacion='pop',elementos=values))
 check(r['elementos']==['20','10'],'pop LIFO')
 status,r=post('/api/pila',dict(tipo=representation,operacion='clear',elementos=values))
 check(r['vacia'] and r['count']==0,'clear')
 for operation in ['pop','peek']:
  status,r=post('/api/pila',dict(tipo=representation,operacion=operation,elementos=[]))
  check(status==400 and 'vacía' in r['error'],'underflow')
status,r=post('/api/pila',dict(tipo='arreglo',operacion='push',elementos=['5','4','3','2','1'],dato='6'))
check(status==400 and 'llena' in r['error'],'overflow')
status,r=post('/api/pila',dict(tipo='enlazada',operacion='push',elementos=['5','4','3','2','1'],dato='6'))
check(status==200 and r['count']==6 and r['capacidad'] is None,'nodos más de cinco')
for payload in [dict(tipo='otro',operacion='pop',elementos=[]),dict(tipo='arreglo',operacion='push',elementos=[],dato=' '),dict(tipo='arreglo',operacion='otro',elementos=[]),dict(tipo='arreglo',operacion='pop',elementos=None)]:
 status,r=post('/api/pila',payload);check(status==400,'validación pila')
for expression,value in [('(6+2)*5-8/4','38'),('10-3-2','5'),('2+3*4','14'),('(2+3)*4','20'),('12.5/2.5+1','6'),('2-5','-3'),('0.1234567890123456789012345678+0','0.1234567890123456789012345678')]:
 status,r=post('/api/calcular',dict(expresion=expression));check(status==200 and r['valor']==value,expression)
 check(bool(r['conversion']) and bool(r['evaluacion']),'trazas')
 check(r['evaluacion'][-1]['pilaCimaPrimero']==value,'último estado')
for expression in ['1/0','(2+3','2(3+4)','-3+2','2^3','', 'a+1','1,5+2','1'*513]:
 status,r=post('/api/calcular',dict(expresion=expression));check(status==400 and 'error' in r,'entrada rechazada')
with urllib.request.urlopen(base+'/') as r:check(r.status==200 and b'Laboratorio de pilas' in r.read(),'panel estático')
print(f'PASS PanelPilas API: {checks} comprobaciones.')
