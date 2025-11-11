# 🧠 Multi-Farmer

## 👥 Conformación del Equipo

| Integrante | Fortalezas | Áreas de oportunidad | Expectativas personales del bloque |
|-------------|-------------|----------------------|------------------------------------|
| Jonathan Roman Velasco | Implementacion de algoritmos, C++, Linux, Control de versiones de Git, Liderazgo | Uso de Unity, Conocimientos de entrenamiento de agentes | Entender los sistemas autonomos, Mejorar el uso de sistemas de control de Git en proyectos mas grandes |
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
- **Gestión de tareas:** Trello / GitHub Projects  
- **Control de versiones:** Git (flujo de ramas: `main`, `develop`, `usuario/feature`)

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
| Agente de Monitoreo (Robot Móvil) | Recorre el invernadero capturando imágenes y datos espectrales | **Reactivo** | Detecta estímulos del entorno y reacciona para recolectar información y evitar obstáculos. |
| Agente de Análisis (IA de Diagnóstico) | Procesa la información visual para detectar anomalías | **Deliberativo** | Usa redes neuronales para identificar patrones y toma decisiones basadas en creencias y metas. |
| Agente Coordinador (Supervisor Híbrido) | Coordina a los agentes y comunica las acciones al humano | **Híbrido** | Combina reacción inmediata ante alertas y planificación deliberativa para distribuir tareas. |
| Agente Humano (Operario) | Recibe notificaciones y ejecuta acciones físicas | — | Representa la interacción humano-sistema y valida decisiones. |

---

## 🧱 Componentes Arquitectónicos

### 🔹 Agente Reactivo (Robot de Monitoreo)
**Capas:**
- **Layer 0: Evitar Obstáculos**  
   IF DetectaObstaculoFrontal() AND DistanciaObstaculo() <= 1m  
   THEN Detener() AND Girar(ángulo) AND Avanzar()

- **Layer 1: Recolectar Datos Críticos (Alta prioridad sensorial)**  
   IF CambiosEspectralesSignificativos() OR VariaciónLuzBrusca() OR DetectaPlaga()  
   THEN AjustarPosición() AND CapturarImagen() AND RegistrarEspectro()

- **Layer 2: Recolectar Datos Regulares**  
   IF TiempoDesdeÚltimaCaptura() > t AND NO DetectaAnomalía()  
   THEN CapturarImagen() AND RegistrarEspectro()

- **Layer 3: Reubicar para Mejor Observación**  
   IF ImagenDifusa() OR SeñalEspectralDébil()  
   THEN Reposicionar() AND ReintentarCaptura()

- **Layer 4: Patrullaje / Vagar Controlado**  
   IF NO DetectaObstaculos() AND NO DetectaAnomalías()  
   THEN AvanzarRuta()  
   ELSE AjustarTrayectoria()

### 🔹 Agente Deliberativo (Análisis por IA)
- **Creencias (B):** Base de datos de imágenes y patrones de enfermedades.  
- **Deseos (D):** Mantener cultivos saludables y reducir infecciones.  
- **Intenciones (I):** Clasificar anomalías y enviar alertas oportunas al supervisor.  

### 🔹 Agente Híbrido (Coordinador)
- **Capas Reactivas:** Responde a alertas de anomalía en tiempo real.  
- **Componentes BDI:** Planifica la asignación de tareas y analiza la severidad del problema.  
- **Integración:** Combina reactividad (alertas) y deliberación (gestión de acciones globales).

---

## 📅 Plan de Trabajo

### 📌 Actividades Pendientes

| Actividad | Descripción | Fecha estimada | Responsable | Estado | Esfuerzo estimado |
|------------|-------------|----------------|--------------|---------|--------------------|
| Revisión bibliográfica sobre virus y plagas | Investigación de fuentes académicas | 18/11/2025 | [Nombre 1] | Pendiente | 5 h |
| Definición de agentes y arquitectura general | Diseño de roles y relaciones | 20/11/2025 | [Nombre 2] | Pendiente | 4 h |
| Creación del entorno de simulación 3D | Modelado del invernadero y los robots | 25/11/2025 | [Nombre 3] | Pendiente | 6 h |
| Entrenamiento preliminar de modelo de detección | Prueba con dataset de hojas infectadas | 02/12/2025 | [Nombre 1] | Pendiente | 8 h |

### 🧾 Actividades para la Primera Revisión

| Actividad | Responsable | Fecha de realización | Intervalo de esfuerzo |
|------------|-------------|-----------------------|-----------------------|
| Definición formal de la arquitectura multiagente | [Nombre 2] | 20/11/2025 | 3–5 h |
| Creación del repositorio y estructura de carpetas | [Nombre 3] | 18/11/2025 | 2–3 h |
| Redacción de la propuesta y descripción de agentes | [Nombre 1] | 22/11/2025 | 4–6 h |

---

## 📚 Aprendizaje Adquirido Del Equipo

En esta etapa pudimos realizar con éxito el aterrizaje del reto para poder organizar en tiempo y forma las siguientes actividades para lograr con éxito a la solución del problema planteado. De igual forma empezar a documnetar con la herramienta Markdown y mantener un formato limpio y con buena estructura.

---

📅 **Versión del documento:** v1.0  
✏️ **Última actualización:** 10/11/2025  
👨‍💻 **Equipo:** Nightgaunts
