<div align="center">

# 🎮 CocoDoogy  
### 소코반 규칙을 기반으로 한 3D 모바일 퍼즐 게임
<a href="https://youtu.be/UCx8Xok3t5I">
  <img width="100" height="100" alt="Youtube_logo"
    src="https://github.com/user-attachments/assets/2aa6f449-7ffa-4dd2-9086-232f5499456f" />
</a>

<br><br>

<table>
  <tr>
    <td align="center" width="33%">
      <img alt="Play_img" src="https://github.com/user-attachments/assets/dd34c8ad-a9d2-4054-9770-303827f1ca54" width="100%" />
      <br/>
      <b>퍼즐 플레이</b>
    </td>
    <td align="center" width="33%">
      <img alt="Env_img" src="https://github.com/user-attachments/assets/1d9420ef-6cd9-45b8-8d9c-3cdee9d1de95" width="100%" />
      <br/>
      <b>기믹</b>
    </td>
    <td align="center" width="33%">
      <img alt="UI_img" src="https://github.com/user-attachments/assets/1101aa67-0672-49a3-81bf-6977f11d6ee3" width="100%" />
      <br/>
      <b>도움말</b>
    </td>
  </tr>
</table>

<br>

🐶 **기업협약 프로젝트로 진행된 소코반 기반 모바일 퍼즐 게임**  
🐶 **다양한 환경·상호작용 기믹을 결합한 3D 퍼즐 플레이가 특징**

</div>

<br><br><br>

---

## 📋 Table of Contents

- [기술 스택(Tech Stack)](#tech-stack)
- [개요(Overview)](#overview)
- [아키텍처(Architecture)](#architecture)
- [설계 포인트(Design Notes)](#point)
- [핵심 시스템(Core Systems)](#core-systems)
  - [플레이어(Player Movement & Joystick)](#player-movement--input)
  - [푸셔블(PushableObjects (Puzzle Rule Core))](#pushableobjects-puzzle-rule-core)
  - [동물 친구들"멧돼지, 거북이, 버팔로" & 환경(Animal Friends System & Environment)](#animal-friends-system--environment)
  - [시그널 시스템(Signal System)](#signal-system)
- [공유 환경 시스템(Shared Environment Systems)](#shared-systems)
  - [충격파(Shockwave System)](#shockwave)
  - [범위 시각화(Range Visualization)](#ringrange)
- [기타 시스템(Supporting Systems)](#support-systems)
  - [보물 & 스테이지 UI(Treasure & Stage UI)](#treasure--stage-ui)
  - [게임 정보(Game Info System)](#info-system)
  - [카메라(Camera & Player Experience)](#camera--ux)
  - [빌드(Build & Tooling)](#tooling)
- [개발자(Developer)](#developer)
  
<br><br>

---

<a id="tech-stack"></a>
## 🛠️ Tech Stack

- **Language :** C#  
- **Engine :** Unity 6
- **Version Control :** GitHub (Fork 기반 협업)

<br>

---

<a id="overview"></a>
## 🎯 Overview

<strong>CocoDoogy(코코두기)</strong>는 소코반 규칙을 기반으로 한 **그리드 퍼즐 게임**으로, 플레이어 이동과 다양한 환경·상호작용 기믹을 결합해 퍼즐 규칙을 확장한 **3D 모바일 퍼즐 게임**입니다.

- 플랫폼: Android  
- 개발 엔진: Unity 6
- 개발 기간: 2025.10.16 ~ 2025.12.09  
- 프로젝트 형태: 기업협약 팀 프로젝트 (개발 6명 / 기획 4명)

> 본 README에는 팀 프로젝트 중 제가 맡은 **퍼즐 규칙 시스템, 플레이어 이동 등 인게임** 파트가 정리되어 있습니다.

<br><br>

---

<a id="architecture"></a>
## 🐶 System Architecture

### 규칙 중심 Pushable 아키텍처

#### 📂 Source Entry
- [`/Assets/_Proj/Scripts`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts)

<div align="center"><a href="https://github.com/user-attachments/assets/3b85ed4a-4639-4892-b1b1-8e0a243a2698"><img width="800" alt="doogy pushable architecture" src="https://github.com/user-attachments/assets/3b85ed4a-4639-4892-b1b1-8e0a243a2698" /></a></div>

<br>

---

### Pushables 플로우 차트
<div align="center"><a href="https://github.com/user-attachments/assets/8edede77-c887-4846-98fa-8a1ef7a8f60b"><img height="650" alt="doogy pushables flow chart" src="https://github.com/user-attachments/assets/8edede77-c887-4846-98fa-8a1ef7a8f60b" /></a></div>

<br>

---

### 시그널 시스템 아키텍처
<div align="center"><a href="https://github.com/user-attachments/assets/0a1130e8-d521-4e27-b43d-bb0341b873ab"><img width="700" alt="doogy signal system architecture" src="https://github.com/user-attachments/assets/0a1130e8-d521-4e27-b43d-bb0341b873ab" /></a></div>

<br>

---

<a id="point"></a>
## ⚙️ Design Notes
- **Strategy/Interface 기반 설계로 기믹 간 결합도 최소화**  
- **퍼즐 규칙의 일관성과 안정성을 최우선으로 유지**  
- **모바일 환경을 고려한 입력 / 카메라 / UI 흐름 설계**  
- **환경·충격·시그널·UI 시스템의 책임 명확화**  

<br><br>

---

<a id="core-systems"></a>
## 🏅 Core Systems

<!-- #### 🔗 Detailed Design & Flow
<a href="https://www.notion.so/CocoDoogy-2e7eddbae78d80649c7fcb5607244823?source=copy_link"><img with = "20" height="20" alt="notion icon" src="https://noticon-static.tammolo.com/dgggcrkxq/image/upload/v1570106347/noticon/hx52ypkqqdzjdvd8iaid.svg" /> 노션 기술문서 링크</a>-->

<a id="player-movement--input"></a>
### 🎮 Player Movement & Input

#### 📂 Code Reference
- [`/Scripts/Player`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Player)

💡 **모바일 환경을 고려한 입력·이동 시스템으로, 플레이어 조작과 퍼즐 규칙 간 결합도를 최소화하도록 설계**

- **주요 기능**
  - 가상 조이스틱 기반 입력 처리
    - 터치 개수에 따른 입력 모드 분기
      - 1손가락: 플레이어 이동
      - 2손가락: 카메라 둘러보기(Look Around) 모드
    - UI 위 터치 입력 차단 및 입력 각도 기반 방향 스냅
  - Rigidbody 기반 이동 파이프라인 구성
  - 퍼즐 상황별 이동 보정을 Strategy 패턴으로 분리
  - 카메라 조작, 상호작용, 탑승(IRider) 상태에 따른 입력 제어

<br>

---

<a id="pushableobjects-puzzle-rule-core"></a>
### 📦 PushableObjects (Puzzle Rule Core)

#### 📂 Code Reference
- [`/Scripts/Gimmicks/Objects`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects)

💡 **코코두기 퍼즐에서 모든 이동 규칙을 단일 파이프라인으로 관리하는 핵심 규칙 시스템**

- **주요 기능**
  - 이동·낙하·적층·충격 반응을 **공통 규칙**으로 처리
  - 플레이어·동물·환경 등 다양한 환경에서도 **동일한 이동 규칙 유지**
  - 규칙 우회 이동 방지로 퍼즐 안정성 확보

<br>

#### [`IPushHandler.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects/IPushHandler.cs)
💡 **플레이어 입력과 퍼즐 규칙을 분리하기 위한 인터페이스**

  - 플레이어는 “밀기 시도”만 전달
  - 실제 이동 판단과 실행은 PushableObjects가 전담

<br>

#### [`IRider.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects/IRider.cs)
💡 **탑승(적층) 구조를 처리하기 위한 인터페이스**

  - 상단 오브젝트를 함께 이동시키기 위한 동기화 처리
  - 이동 규칙을 변경하지 않고 적층 퍼즐 구현 가능

<br>

#### [`PushableBox.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects/PushableBox.cs)
💡 **기본 밀기 퍼즐 오브젝트**

  - PushableObjects의 기본 이동 규칙만을 그대로 따르는 최소 구현체

<br>

#### [`PushableOrb.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects/PushableOrb.cs)
💡 **이동 규칙은 유지하면서, 착지 이벤트를 퍼즐 트리거로 확장한 오브젝트**

  - 낙하 착지 시 충격파를 발생시키는 특수 퍼즐 요소
  - 충격 이벤트를 감지탑 및 문으로 전달
  - 기존 이동 규칙을 수정하지 않고 퍼즐 흐름 확장

<br>

---

<a id="animal-friends-system--environment"></a>
### 🐾 Animal Friends System & Environment

#### 📂 Code Reference
- [`/Scripts/Gimmicks/Animals`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Animals)
- [`/Scripts/Stage/Block/Water`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Stage/Block/Water)

💡 **퍼즐 규칙을 직접 변경하지 않고, 플레이어의 이동 방식과 퍼즐 흐름을 확장하는 상호작용 기반 기믹 시스템**

- **주요 기능**
  - PushableObjects의 이동 규칙을 그대로 사용
  - 규칙을 바꾸지 않고, 규칙을 사용하는 방식만 확장하는 설계
  - 각 동물은 서로 다른 상호작용을 제공하지만, 동일한 이동 규칙 파이프라인을 공유

<br>


#### 🐗 Boar
💡 **직선 돌진을 통해 퍼즐 오브젝트 체인을 이동시키는 돌진·충돌형 기믹**

  - 입력 방향으로 돌진하며 전방 퍼즐 오브젝트 체인을 밀어냄
  - 수평 체인과 적층 구조를 함께 고려한 퍼즐 상호작용 제공
  - 이동 규칙은 PushableObjects에 위임

<br>


#### 🐢 Turtle
💡 **빙판 규칙 기반 연속 이동 + 탑승 구조를 결합한 이동 보조 기믹**

  - 장애물에 부딪힐 때까지 자동 이동하는 슬라이드 퍼즐 기믹
  - 이동 중 상단 오브젝트를 함께 이동시키는 탑승 구조 제공

<br>


#### 🐃 Buffalo
💡 **충격파를 통해 퍼즐 상태 변화를 유도하는 고정형 환경 기믹**

  - 플레이어 상호작용을 트리거로 충격파를 발생
  - 쿨타임 기반 재사용 제한으로 퍼즐 흐름 제어

<br>


#### 🌊 Flow Water (Environment)
💡 **물 타일 위 오브젝트의 주기적인 이동을 유도하는 환경 보조 기믹**

  - 물 타일 위에 놓인 퍼즐 오브젝트를 주기적으로 이동
  - 이동 대상 및 방향 결정은 공통 규칙으로 처리
  - 실제 이동 방식은 전략 패턴으로 분리하여 확장성 확보

<br>

---

<a id="signal-system"></a>
### 🛰️ Signal System

#### 📂 Code Reference
- [`Scripts/Gimmicks/Objects`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects)
- [`/Scripts/Stage/Block`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Stage/Block)

💡 **충격, 감지, 조건 충족 등의 이벤트를 직접적인 참조 없이 시그널로 연결하여 퍼즐 상태 변화를 제어하는 기믹 시스템**

#### 🚦 Signal Interface
💡 **기믹 간 직접 참조 없이 시그널 전달만 표준화하는 연결 규약**

  - 시그널 송신자(Sender)와 수신자(Receiver)를 분리하는 공통 인터페이스
  - 감지탑, 터렛, 스위치 등 다양한 기믹 조합 지원
  - 특정 기믹에 종속되지 않는 시그널 흐름 구성

<br>

#### 🗼 ShockDetectionTower
💡 **충격파(Shockwave)를 시그널로 변환하여 송·수신 모두 수행하는 감지 기믹**

  - 충격파 이벤트를 감지하여 시그널로 변환하는 감지 기믹
  - 쿨타임 기반 중복 감지 방지
  - 인접 감지탑으로 시그널을 릴레이하여 전파
  - Door 등 시그널 수신 기믹과 연결되어 상태 변화 유도

<br>

#### 💿 Turret
💡 **FOV/반경으로 타겟 상태를 판정하고, 감지 결과를 시그널로 송신하는 감시 기믹**

  - 시야각(FOV) 및 반경 기준으로 타겟을 감지하는 감시 기믹
  - 감지 상태에 따라 시그널을 송신하여 퍼즐 조건 제어
  - 감지 범위와 상태를 시각적으로 표현하여 플레이어에게 피드백 제공

<br>

#### 🚪 Door
💡 **시그널 수신에 따라 열림/닫힘을 전환하는 수신형 퍼즐 오브젝트**

  - 시그널 수신에 따라 열림/닫힘 상태가 변경되는 퍼즐 오브젝트
  - 외부 기믹 조건에 의해 상태가 제어됨
  - 조건 충족 시 영구 개방 등 퍼즐 설계 확장

<br>

---

<a id="shared-systems"></a>
## 🌊 Shared Environment Systems

💡 **여러 기믹에서 공용으로 사용되는 환경·보조 시스템으로, 특정 도메인에 종속되지 않고 퍼즐 상호작용을 지원하는 역할 담당**

<a id="shockwave"></a>
### ⚫ Shockwave Systems

#### [`Shockwave.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Shockwave.cs)
💡 **반경 내 오브젝트에 충격파를 전달하는 공용 시스템**

  - 반경 내 퍼즐 오브젝트에 충격(리프트) 효과를 전달하는 공용 환경 시스템
  - 적층 구조를 고려한 충격 전파 처리로 퍼즐 안정성 유지
  - 차폐(Occlusion) 옵션을 통한 충격 전파 제어

<br>

#### [`ShockPing.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Objects/ShockPing.cs)
💡 **충격파 발생을 감지탑으로 전달하는 브리지 컴포넌트**

  - 충격파 발생 시 반경 내 감지탑으로 이벤트를 전달하는 중계 컴포넌트
  - 충격파와 감지탑(타워) 간 직접 의존을 제거하는 역할

<br>

<a id="ringrange"></a>
### 🟢 Range Visualization

#### [`RingRange.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Gimmicks/Animals/RingRange.cs)
💡 **감지·상호작용·충격 범위를 시각화하는 공용 렌더링 컴포넌트**

  - 충격, 감지, 상호작용 범위를 시각하기 위한 공용 렌더링 컴포넌트
  - 원형·부채꼴 범위를 지원하여 다양한 기믹에서 재사용
  - 반경 및 각도 변경에 대응하여 실시간으로 시각 요소 갱신

<br>

---

<a id="support-systems"></a>
## 🏅 Supporting Systems

💡 **퍼즐 규칙에는 직접 관여하지 않지만, 플레이 흐름·정보 전달·연출 완성도를 책임지는 보조 시스템**

<a id="treasure--stage-ui"></a>
### 💎 Treasure & Stage UI

#### 📂 Code Reference
- [`Treasure.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Stage/Block/Treasure.cs)
- [`/Assets/_Proj/Script/UI/Option`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/UI/Option)

💡 **퍼즐 목표 인식 → 보상 → 다음 행동 유도의 UX 흐름을 담당**

  - 스테이지 내 핵심 목표 요소(보물)의 획득 상태를 관리
  - 획득 시 입력 제어 및 UI 피드백을 통해 플레이 흐름을 명확히 전달
  - 스테이지 진입/인게임 상황에 맞춰 현재 스테이지 정보를 표시

<br>

---

<a id="info-system"></a>
### 🧭 Game Info System

#### [`GameInfoPopup.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/UI/Option/GameInfoPopup.cs)
💡 **초기 1회 튜토리얼 이후, 플레이 중 자율 학습을 유도하는 정보 시스템**

  - 매뉴얼 데이터를 기반으로 도움말 UI를 동적으로 구성
  - 탭 구조를 통해 퍼즐 기믹, 조작 방법, 규칙 등을 안내
  - 최초 1회 필수 튜토리얼 이후, 필요 시 언제든 확인 가능한 도움말 구조
  - 기획 데이터 변경 시 코드 수정 없이 도움말 확장 가능

<br>

---

<a id="camera--ux"></a>
### 🎥 Camera & Player Experience

#### [`CamControl.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Camera/CamControl.cs)
💡 **플레이어 추적, 둘러보기, 스테이지 카메라 워킹을 담당하는 카메라 컨트롤러**

  - 플레이어 추적, 자유 시점, 연출용 카메라 워킹을 상황에 따라 전환
  - 입력 상태 및 게임 흐름에 따라 카메라 제어 방식 분리
  - 퍼즐 플레이 방해 없이 공간 인식과 몰입도를 유지하도록 설계

<br>

---
<a id="tooling"></a>
### 🔁 Build & Tooling

#### [`AutoVersionIncrement.cs`](https://github.com/devschnee/CocoDoogy-Public/blob/main/Assets/_Proj/Scripts/Editor/AutoVersionIncrement.cs)
💡 **개발 편의성과 빌드 기록을 위한 내부 도구**

  - 빌드 시점마다 게임 버전 자동으로 증가
  - 수동 버전 관리로 인한 실수 방지
  - 팀 협업 환경에서 빌드 결과 추적 용이성 확보

<br>

---

<a id="developer"></a>
## 👨‍💻 개발자
<div align="center">

**김현지**

<br>

<a href="https://github.com/devschnee">
  <img src="https://img.shields.io/badge/devschnee-blue?style=for-the-badge&logo=GitHub&logoColor=ffffff&label=GitHub&labelColor=Black"/>
</a>

<br><br>

**CocoDoogy – 퍼즐 규칙과 시스템 설계를 중심으로 완성도를 추구한 프로젝트**

퍼즐 규칙 시스템, 플레이어 이동, 환경 기믹 아키텍처 설계 및 구현  
모바일 환경 기준 UX 검수 및 입력·피드백 흐름 개선

</div>