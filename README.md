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

## 💪 Expectativas del Equipo
- Desarrollar un sistema funcional con agentes colaborativos.
- Fortalecer habilidades de planificación y trabajo ágil.
- Mantener comunicación constante.
- Profundizar en agentes con aplicación profesional.

## 🤝 Compromisos del Equipo
- Cumplir tiempos del plan de trabajo.  
- Documentar avances.  
- Apoyo entre compañeros.  
- Respeto y enfoque.

---

## 🧰 Herramientas Colaborativas
- **Repositorio:** https://github.com/JRV-XVI/multi-farmer  
- **Comunicación:** Discord / WhatsApp  
- **Gestión de tareas:** Trello / GitHub Projects  
- **Control de versiones:** Git con flujo `main` → `develop` → `feature/usuario`

---

# 🚀 Reto del Proyecto

Los cultivos agrícolas dependen de detección temprana de plagas y enfermedades. El virus **Rugoso del Tomate** afecta cultivos como tomate y pimiento, propagándose rápidamente y causando pérdidas críticas.

## 🌱 Problema
Los síntomas aparecen tarde y son difíciles de identificar, generando eliminación masiva de plantas.

## 💡 Solución Propuesta
Un **sistema multiagente autónomo** que:
1. Monitorea continuamente plantas.
2. Detecta signos de estrés o enfermedad.
3. Clasificacion de anomalías.
4. Decide acciones: tratamiento, purga o intervención humana.
5. Coordina agentes para ejecución eficiente.

---

# 🧩 Refactorización de Agentes

## 🔹 Agente Explorador (Híbrido)

**Rol principal:**  
Recorre el huerto analizando plantas para identificar posibles enfermedades.

**Responsabilidades:**  
- Analizar plantas con visión y sensores.  
- Registrar coordenadas exactas de plantas enfermas.  
- Calcular nivel de severidad inicial (índice espectral / modelo IA).  
- Enviar reporte estructurado al Agente Coordinador.  

**Arquitectura:**  
- Reactivo para navegación (evitar obstáculos, patrullaje).  
- Deliberativo para interpretación de imágenes y estimación de severidad.  

---

## 🔹 Agente Recolector (Reactivo)

**Rol principal:**  
Recolecta toda la fruta sana siguiendo un camino eficiente.

**Responsabilidades:**  
- Recibir lista de coordenadas de plantas sanas.  
- Generar ruta optimizada (heurística TSP / distancia mínima).  
- Navegar evitando obstáculos.  
- Recolectar frutos y llevarlos al punto de acopio.  

**Arquitectura:**  
- Reactivo puro: comportamiento basado en estímulos y prioridades.  

---

## 🔹 Agente Purgador (Reactivo)

**Rol principal:**  
Eliminar plantas enfermas y desechar residuos de manera controlada.

**Responsabilidades:**  
- Recibir todas las coordenadas de plantas enfermas.  
- Optimizar ruta para eliminación eficiente.  
- Realizar proceso de purga: eliminar planta → embolsado → transporte.  
- Llevar restos al basurero asignado.  

**Arquitectura:**  
- Reactivo puro con alta prioridad de seguridad.  

---

# 🧱 Componentes Arquitectónicos

### Agente Reactivo (Explorador — nivel reactivo)
- Evitar obstáculos.
- Captura de datos ante estímulos espectrales.
- Corrección de posición para mejor lectura.
- Patrullaje controlado.

### Agente Deliberativo (Análisis IA)
- Procesamiento de imágenes.
- Clasificación de anomalías.
- Generación de alertas.

### Agente Coordinador
- Mezcla BDI + reactividad.
- Distribución de tareas entre agentes.
- Supervisión y comunicación con operario humano.

---

## 📅 Plan de Trabajo

[Tablero del Proyecto en GitHub](https://github.com/JRV-XVI/multi-farmer/projects)

## 📚 Aprendizaje Adquirido Del Equipo

En esta etapa pudimos realizar con éxito el aterrizaje del reto para poder organizar en tiempo y forma las siguientes actividades para lograr con éxito a la solución del problema planteado. De igual forma empezar a documnetar con la herramienta Markdown y mantener un formato limpio y con buena estructura.

---

📅 **Versión del documento:** v1.0  
✏️ **Última actualización:** 10/11/2025  
👨‍💻 **Equipo:** Nightgaunts
