# 도화 캐릭터 이동

Unity_LOPMoba의 번호별 분류 방식에 맞춘 프로젝트 자산 구조입니다.

- 01_Scenes/Dev: HeroPlayground 및 개발용 씬.
- 02_Scripts/Gameplay: 캐릭터 이동·카메라 코드.
- 02_Scripts/Editor: PixelLab 애니메이션 에셋 갱신 도구.
- 03_Prefabs/B_Characters: HeroRig 프리팹.
- 03_Shaders: 셰이더.
- 05_Materials: 캐릭터와 연습장 머티리얼.
- 06_Model/Characters/Hero/Textures/PixelLab: 원본 PNG 140장 및 생성 기록.
- 06_Model/Characters/Hero/Sprites/PixelLab: 스프라이트 224개.
- 07_Animation/PixelLab: 8방향 대기·걷기·달리기·대시, 총 32개 클립.
- 07_Animation/PixelLab.controller: 현재 컨트롤러.

위 경로는 모두 Assets/00_DoHwaTown 기준입니다. 공통 렌더링 설정은 Assets/Settings에 둡니다.
DoHwa/Apply PixelLab Motions 메뉴로 현재 자산을 갱신할 수 있습니다.
정면은 94×100, 나머지는 96×100 픽셀. Point 필터, 무압축, 밉맵 없음.
왼쪽 3방향은 오른쪽 방향을 좌우 반전하며 머리 장식도 반전됩니다.

WASD / 방향키: 걷기. Shift: 달리기. Space: 대시.
게임패드: 왼쪽 스틱 이동, 오른쪽 트리거 달리기, 아래쪽 버튼 대시.
