# 🧠 Multi-Farmer

## 👥 Conformación del Equipo

| Integrante | Fortalezas | Áreas de oportunidad | Expectativas personales del bloque |
|-------------|-------------|----------------------|------------------------------------|
| Jonathan Roman Velasco | Implementacion de algoritmos, C++, Linux, Control de versiones de Git, Liderazgo | Conocimientos de entrenamiento de agentes | Entender los sistemas autonomos, Mejorar el uso de sistemas de control de Git en proyectos mas grandes |
| Mario Feng Wu | Uso de Unity, Python, Organización de timepos y tareas, Análisis de datos, Machine Learning | Conocimiento de agentes para automatización de tareas | Comprender el funcionamiento de un agente y emplearlo en un caso cotidiano, así mismo expandir el uso de Unity y C# |
| Luis Fernando Valderrabano | Ciberseguridad, Python, Linux, Servidores, Redes | Organización de tareas, seguridad en la aplicación que se realice | Aprender funcionamiento y arquitectura de agentes para ciberseguridad o pentesting |
| Octavio Sebastián Hernández Galindo | Uso de Unity, Python, C++, Git & GitHub, Documentación | Conocimiento teórico y práctico de agentes, bases de IA | Recibir bases sólidas sobre sistemas de IA y expandir conocimiento de Unity y C# |
| Ángel Gabriel Camacho Pérez | Uso de Unity, C++, diseño OO, Github, algoritmos | Ciberseguridad, Python, Machine Learning | Espero mejorar mis conocimientos de Unity, aplicar algoritmos aprendidos en clase y aprender a modelar en 3D. |
| José Pedro Gastélum Beltrán | Unity, C++, Git y Github, Python | Profundizar en entrenamiento y comportamiento de agentes, mejorar optimización de algoritmos en entornos complejos | Comprender el funcionamiento de agentes y emplearlos para automatización, adquirir conocimientos de Unity y C# |

### 💪 Expectativas del Equipo
- Desarrollar un sistema funcional con agentes colaborativos.
- Fortalecer nuestras habilidades en planificación y trabajo de manera ágil.
- Mantener una comunicación constante y efectiva.
- Profundizar en el desarrollo de agentes para aplicarlos en el area profesional.

### 🤝 Compromisos del Equipo
- Cumplir con los tiempos establecidos en el plan de trabajo.  
- Documentar correctamente cada avance.  
- Apoyar a los compañeros en tareas críticas o retrasadas.
- Mantener un equipo de trabajo con respeto y enfocado.

---

## 🧰 Creación de Herramientas de Trabajo Colaborativo
- **Repositorio en GitHub:** https://github.com/JRV-XVI/multi-farmer
- **Herramienta de comunicación:** Discord / Whatsapp
- **Gestión de tareas:** GitHub Projects  
- **Control de versiones:** Git (flujo de ramas: `main`, `develop`, `<user>/*feature`)

---

## 🚀 Descripción del Reto a Desarrollar

Los cultivos agrícolas representan cerca del 80% de la dieta humana. En México, frutos de alta rotación como la fresa o el pepino deben cosecharse en ventanas cortas para preservar su valor comercial.  
Actualmente, la detección de plagas y enfermedades depende de inspecciones visuales tardías, generando pérdidas de hasta 40% de la producción mundial (FAO, 2022).  

### 🌱 Problema específico
En cultivos como tomate o pimiento, el virus **Rugoso del Tomate** se propaga rápidamente mediante el contacto con herramientas o manos contaminadas. Los síntomas son tardíos y difíciles de identificar visualmente, lo que provoca la eliminación masiva de plantas.

### 💡 Solución propuesta
Desarrollar un **sistema multiagente autónomo** capaz de:
1. Monitorear continuamente las plantas dentro de un invernadero mediante agentes móviles / estaticos.  
2. Detectar tempranamente signos de estrés o enfermedad mediante visión por computadora y sensores multiespectrales.  
3. Transmitir los datos a un agente deliberativo de decisión que determine acciones de manejo.  
4. El agente decidira en base a su entorno y desición sobre las medidas a ejecutar (eliminación o tratamiento).
5. Si se necesita intervención humana, estara el operario (Agente Humano) para acciones especiales.

**Objetivo general:**  
Mejorar la eficiencia de detección y respuesta ante anomalías en cultivos agrícolas, reduciendo pérdidas y uso innecesario de recursos con el diseño de un sistema multiagente.

---

## 🧩 Identificación de los Agentes Involucrados

| Agente | Rol / Función | Tipo de arquitectura | Descripción breve |
|---------|----------------|----------------------|-------------------|
| **Agente Explorador** | Recorre el huerto analizando plantas para identificar posibles enfermedades. | **Híbrido** | Combina navegación reactiva con análisis deliberativo mediante visión e IA para detectar y reportar plantas enfermas. |
| **Agente Recolector** | Recolecta la fruta sana siguiendo una ruta eficiente. | **Reactivo** | Opera mediante estímulos y respuestas, optimizando su trayecto y evitando obstáculos mientras recolecta los frutos. |
| **Agente Purgador** | Elimina plantas enfermas y desecha residuos de manera controlada. | **Reactivo** | Utiliza una arquitectura de respuesta directa con prioridad de seguridad para realizar procesos de purga y transporte de desechos. |

---

## 🧱 Componentes Arquitectónicos

### 🔹 Agente Híbrido (Explorador)

**Capas Reactivas:**
- **Layer 0: Evitar Obstáculos**  
  `IF DistanciaObstaculoFrontal() < 1m THEN Detener() AND Girar(ángulo) AND Avanzar()`

- **Layer 1: Patrullaje del Huerto**  
  `IF NO DetectaPlanta() THEN Vagar() AND BuscarNuevaPlanta()`

- **Layer 2: Análisis de Planta**  
  `IF DetectaPlanta() THEN CapturarImagen() AND AnalizarSeveridad() AND RegistrarCoordenadas()`

**Componentes Deliberativos (BDI):**
- **Creencias (B):** Estado actual del terreno, coordenadas de plantas enfermas, historial de análisis previos.  
- **Deseos (D):** Identificar y reportar todas las plantas con signos de enfermedad.  
- **Intenciones (I):** Procesar imágenes, estimar severidad y enviar reporte estructurado.  

**Integración:**  
Combina una capa reactiva para desplazamiento y evasión con una capa deliberativa para interpretación visual y generación de reportes automáticos.

---

### 🔹 Agente Reactivo (Recolector)

**Capas:**
- **Layer 0: Evitar Obstáculos**  
  `IF DistanciaObstaculoFrontal() < 1m THEN Detener() AND Girar(ángulo) AND Avanzar()`

- **Layer 1: Navegación hacia Planta Sana**  
  `IF RecibioCoordenada() THEN CalcularRutaOptima() AND AvanzarRuta()`

- **Layer 2: Recolección de Fruta**  
  `IF LlegóAPlanta() AND DetectaFrutoSano() THEN RecolectarFruta() AND TransportarAlAcopio()`

**Descripción general:**  
Su comportamiento se basa en estímulo-respuesta con una prioridad en eficiencia de movimiento, sin planificación compleja.  
Aplica heurísticas locales para minimizar tiempo de recolección y consumo energético.

---

### 🔹 Agente Reactivo (Purgador)

**Capas:**
- **Layer 0: Evitar Obstáculos**  
  `IF DistanciaObstaculoFrontal() < 1m THEN Detener() AND Girar(ángulo) AND Avanzar()`

- **Layer 1: Navegación hacia Planta Enferma**  
  `IF RecibioCoordenada() THEN CalcularRutaSegura() AND AvanzarRuta()`

- **Layer 2: Purga y Eliminación**  
  `IF LlegóAPlanta() AND ConfirmadaComoEnferma() THEN EliminarPlanta() AND EmbolsarResiduos() AND TransportarABasurero()`

**Descripción general:**  
Funciona bajo una arquitectura **reactiva con alta prioridad de seguridad**, garantizando que las acciones de eliminación y transporte de residuos se realicen sin interferir con los demás agentes ni comprometer el entorno.

---

## 📅 Plan de Trabajo

[Tablero del Proyecto en GitHub](https://github.com/JRV-XVI/multi-farmer/projects)

---

## 📚 Aprendizaje Adquirido Del Equipo

En esta etapa pudimos realizar con éxito el aterrizaje del reto para poder organizar en tiempo y forma las siguientes actividades para lograr con éxito a la solución del problema planteado. De igual forma empezar a documnetar con la herramienta Markdown y mantener un formato limpio y con buena estructura.

---

📅 **Versión del documento:** v1.0  
✏️ **Última actualización:** 10/11/2025  
👨‍💻 **Equipo:** Nightgaunts
