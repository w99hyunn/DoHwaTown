# 공용 uGUI 이미지 모듈

## 구성
- `Surface.png` / `Surface9Slice.asset`: 글자나 아이콘이 없는 흰색 배경 이미지. 가운데만 늘리고 모서리는 고정한다.
- `Symbols.png` / `ConfirmIcon.asset` / `CancelIcon.asset`: 독립 O/X 스프라이트. 배경과 파일·렌더러가 분리되어 있다.
- `WhiteTheme.asset`: 공용 이미지, 글자/상태 색, 모서리 크기, 아이콘 크기/여백, 전환 속도.
- `ConfirmButton.prefab`, `CancelButton.prefab`: 공용 버튼. 클릭 이벤트는 빈 상태이므로 사용하는 화면에서 연결한다.
- `Panel.prefab`, `ItemSlot.prefab`: 같은 9-slice 이미지를 사용하는 크기 조정 가능한 기본 표면. 이 둘은 제공용 프리팹이며 기존 화면 전체를 교체하지 않았다.

버튼 계층은 `Button → Frame / Background / Icon / Label`이다. 루트의 투명 Image는 입력 영역이고, 자식 Image와 TMP는 raycastTarget=false다. 버튼 폭을 바꿔도 Icon은 24×24이며 Preserve Aspect를 유지한다. 텍스트는 TMP로 별도 렌더링한다. 이미지에 글자를 굽지 않는다.

## 사용
1. Canvas 아래에 ConfirmButton 또는 CancelButton 프리팹을 배치한다.
2. 루트 RectTransform의 너비/높이를 지정한다. 권장 높이 52–64, 한글 짧은 라벨 기준 최소 너비 168.
3. `UiModuleButton.SetLabel(...)`, `SetIcon(sprite)`, `SetTheme(theme)`으로 내용/테마를 교체한다. 일반 Unity Button.onClick으로 기능을 연결한다.
4. 테마를 에디터에서 수정했다면 UiModuleButton 컴포넌트 메뉴의 Apply Theme로 미리보기를 갱신한다. 플레이 시작 시에도 테마가 적용된다.
5. 비활성은 Button.interactable=false로 지정한다. 별도의 ColorTint나 스케일 애니메이션을 겹치지 않는다.

## 현재 적용 범위
Login의 확인 3개·취소·종료, Ingame의 정렬·상세 실행·수첩 실행 2개·설정 돌아가기, 총 10개에 공용 테마와 컴포넌트를 연결했다. 기존 클릭 이벤트는 유지했다. 기존 씬의 버튼은 테마 공유 방식이며 새 프리팹의 연결 인스턴스로 바꿨다고 주장하지 않는다.

## 이미지 제작
Built-in image_gen으로 배경과 심볼을 각각 별도 생성했다. 원본 PNG를 보존하고 Unity의 스프라이트 영역과 9-slice 메타데이터만 설정했다.

배경 프롬프트: One blank horizontal button background only, transparent canvas, clean white / very pale neutral ivory face, smooth uniform surface, subtle cool-gray outline, restrained rounded corners, straight sides, Unity 9-slice. No icons, text, badges, diamonds, grain, metal, bevel, gold, navy, shadows or glow.

심볼 프롬프트: Exactly two isolated monochrome white UI symbols on transparent canvas: hollow O ring and symmetric rounded-stroke X. Equal visual weight, square bounds, separately sliceable and tintable Unity sprites. No button backgrounds, badges, text, gradients, texture, shadow or glow.

이전 일체형 남색/금색 이미지는 새 버튼에서 사용하지 않는다.

## 검증
- ConfirmButton 프리팹을 실행 중 생성해 폭 168/320/504에서 아이콘 24×24, Preserve Aspect 및 짧은 한글 라벨 overflow 없음 확인.
- SetIcon/SetLabel 독립 교체 및 프리팹의 화면 종속 클릭 연결 없음 확인.
- 로그인 설정·공지·언어 확인과 종료창 취소를 Raycast hit target + pointer click으로 닫기 검증.
- 로그인/인게임의 대표 버튼 normal/hover/pressed/disabled 실행 화면은 Tools/UIReview/States에 저장.

## 2026-10-17 reference button revision
- Direct visual source: Tools/UIReview/References/CZN_Buttons_User.png (user supplied).
- ReferenceButtons.png: generated from the source style; no baked text/icons. White primary and charcoal secondary, rounded ends, double rim, short shadow and low-contrast geometric end pattern.
- PatternPrimary.asset / PatternSecondary.asset: nine-sliced, native texture aspect preserved. Side borders retain decorated endcaps; center stretches.
- WhiteTheme.asset is shared by 10 scene buttons and ConfirmButton / CancelButton prefabs. Polygon artwork is no longer referenced by the theme or close controls.
- Widths 168/320/504 at height 64 captured in Tools/UIReview/States/ButtonWidthAudit.png. Actual scene captures and reference evidence are in Tools/UIReview/DesignComparison.html.
- Hover/pressed are original project tint transitions, not claimed as reference animation. Icon and TMP label remain independent.
