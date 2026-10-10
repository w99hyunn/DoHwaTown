# 도화정 캐릭터 미리보기

현재 신규 분리형 제작본은 **DohwaV4**다. 개발 씬 `01_Scenes/Dev/DohwaMovementPreview.unity`, 프리팹 `03_Prefabs/B_Characters/DohwaV4.prefab`, 파츠와 아이템 `06_Model/Characters/DohwaV4`를 사용한다. 이전 통짜 Hero와 ModularHero 자산은 v4 제작 기준이 아니다.

8방향 독립 대기 8장·걷기 64장, 네이티브 96×96, 발 원점 (48,88), Point/무압축/밉맵 없음. 머리·상의·하의·신발·장식 슬롯을 분리했다. 자세마다 완전한 기본몸체와 긴 머리에 가려진 의상을 복원했고 실제 코디 아이템 데이터로 같은 프레임을 교체한다.

WASD/방향키 걷기, Shift 빠른 이동, Space 대시. Shift/대시는 걷기 원화를 재사용한다. 1~5 코디 슬롯 착용/해제, 6 상의 아이보리/장미색 교체, 0 기본 코디. 게임패드는 기존 왼쪽 스틱/오른쪽 트리거/아래 버튼 입력을 사용한다.

원화·검수 이미지·상세 제작 규격은 `Tools/CharacterPipeline/production/eight-way-v4`에 있다. 오프라인 파츠 합성과 파일 참조 확인을 수행한다. Editor import·Play Mode·셰이더 런타임은 미검증이며 빌드·컴파일 확인은 실행하지 않았다.
