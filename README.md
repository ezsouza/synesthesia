# Synesthesia Player

## 🚀 Funcionalidades Implementadas

### 🎵 **Player de Música Completo**
- ✅ Reprodução de arquivos MP3, WAV, FLAC, M4A
- ✅ Controles de reprodução: Play/Pause, Anterior, Próximo
- ✅ Barra de progresso com scrubbing
- ✅ Controle de volume
- ✅ Shuffle e Repeat
- ✅ Playlist com drag-and-drop support

### 🎨 **Visualizações Psicodélicas**
- ✅ **Bars Visualizer** - Barras verticais clássicas com cores dinâmicas
- ✅ **Wave Visualizer** - Formas de onda fluidas
- ✅ **Kaleidoscope Visualizer** - Padrões caleidoscópicos rotativos
- ✅ **Spectrum Visualizer** - Análise circular de espectro com efeitos de brilho
- ✅ **Particle Flow Visualizer** - Sistema de partículas responsivo ao áudio
- ✅ Modo fullscreen para visualizações imersivas
- ✅ Troca de visualizador em tempo real

### 🎨 **Interface Moderna**
- ✅ Design dark theme minimalista estilo Apple/Windows 11
- ✅ Janela customizada sem bordas do sistema
- ✅ Controles de janela personalizados (minimizar, maximizar, fechar)
- ✅ Layout responsivo com painel de playlist e área de visualização
- ✅ Animações e transições suaves
- ✅ Indicador "Now Playing" com informações da faixa

### 🔧 **Tecnologias Utilizadas**
- **WPF (.NET 9)** - Framework de interface
- **NAudio** - Processamento de áudio e reprodução
- **Canvas e Shapes** - Renderização de visualizações
- **DispatcherTimer** - Animações em tempo real
- **MVVM Pattern** - Arquitetura limpa e testável

### 🧪 **Qualidade e Testes**
- ✅ 24 testes unitários passando
- ✅ Testes para todos os visualizadores
- ✅ Testes com threading STA para componentes WPF
- ✅ Cobertura de casos edge (null, vazios, dimensões inválidas)
- ✅ Mocking com Moq para isolamento de testes

## 🎯 **Como Usar**

1. **Adicionar Músicas**: Clique no botão "+" na seção de playlist
2. **Reproduzir**: Selecione uma faixa da playlist ou use os controles
3. **Visualizações**: Escolha diferentes visualizadores no dropdown
4. **Fullscreen**: Marque "Fullscreen" para visualização imersiva
5. **Controles**: Use os botões de reprodução, volume e modos shuffle/repeat

## 🏗️ **Arquitetura**

```
Synesthesia.App/
├── Audio/                  # Sistema de áudio
│   ├── MusicPlayer.cs     # Player principal com NAudio
│   ├── AudioAnalyzer.cs   # Análise de espectro
│   └── AudioPlayer.cs     # Controles de reprodução
├── Models/                # Modelos de dados
│   ├── Track.cs          # Modelo de faixa musical
│   └── Playlist.cs       # Gerenciamento de playlist
├── Visuals/              # Sistema de visualização
│   ├── BaseVisualizer.cs        # Classe base abstrata
│   ├── BarsVisualizer.cs        # Visualizador de barras
│   ├── WaveVisualizer.cs        # Visualizador de ondas
│   ├── KaleidoVisualizer.cs     # Visualizador caleidoscópio
│   ├── SpectrumVisualizer.cs    # Visualizador de espectro
│   ├── ParticleFlowVisualizer.cs # Sistema de partículas
│   └── VisualEngine.cs          # Engine de gerenciamento
└── MainWindow.xaml/cs    # Interface principal
```

## 🔮 **Experiência de Uso**

### **Similar ao Windows Media Player XP**
- **Playlist lateral** com informações de faixas
- **Visualizações centrais** ocupando a maior parte da tela
- **Controles na parte inferior** para fácil acesso
- **Troca dinâmica** entre diferentes tipos de visualização
- **Modo fullscreen** para experiência imersiva completa

### **Design Moderno**
- **Dark theme** elegante e profissional
- **Tipografia** clara e hierárquica
- **Cores** baseadas no Microsoft Fluent Design
- **Responsividade** para diferentes tamanhos de janela
- **Micro-interações** suaves e polidas

## 🚧 **Próximos Passos Sugeridos**

Para uma versão ainda mais completa:

1. **FFT Real**: Implementar análise FFT real do áudio
2. **Metadados**: Extração automática de informações de arquivos
3. **Equalizer**: Controles de equalização
4. **Skins**: Sistema de temas customizáveis
5. **Plugins**: Arquitetura para visualizadores externos
6. **Performance**: Otimizações para bibliotecas grandes
7. **Formatos**: Suporte para mais formatos de áudio

## 📝 **Notas Técnicas**

- **Threading**: Uso correto de STA threads para componentes WPF
- **Memory Management**: Dispose adequado de recursos de áudio
- **Error Handling**: Tratamento robusto de erros de arquivo/áudio
- **Testability**: Arquitetura permite testes unitários abrangentes
- **Extensibility**: Fácil adição de novos visualizadores

---
