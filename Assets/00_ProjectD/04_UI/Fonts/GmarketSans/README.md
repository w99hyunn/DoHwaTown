# Gmarket Sans Medium

공식 배포: https://corp.gmarket.com/fonts/
다운로드: https://corp.gmarket.com/fonts/GmarketSansOTF.zip
공식 페이지의 배포 조건: SIL Open Font License.

2026-10-17 클라이언트 `C:/Kakaogames/DKBI/client/`의 Addressables 폰트 번들에서 확인한 패밀리/스타일: Gmarket Sans / Medium.
공식 OTF와 게임에 포함된 Font 데이터의 SHA-256이 동일하다:
`39845e70b83555398b0710e85fa389702757dcb22a7241d5a196e90bc23ac9c8`

Project_D용 TMP 아틀라스는 공식 원본으로 새로 생성한다. 36pt, padding 4, SDF16, 2048 정사각형 다중 페이지. 지정 범위 `32-126,44032-55203,12593-12643,8200-9900` 중 원본 폰트에 있는 글리프만 포함한다. 원본에 없는 코드는 Tools/UIReview/GmarketMissingCodepoints.txt에 기록한다.

레이아웃과 글자 크기는 유지하고 Splash/Login/Ingame의 글꼴 및 TMP 기본 폰트를 교체했다. HUD 그림자는 새 폰트 아틀라스를 사용하는 별도 머티리얼로 연결한다.

적용 결과: Splash 1개, Login 32개, Ingame 157개. TMP 캐릭터 테이블 11,621개, 활성 아틀라스 5페이지, asset 46,991,696바이트. 지정 범위에서 원본 폰트가 제공하지 않는 코드포인트 1,392개는 별도 기록했다. 한글 완성형 11,172자는 모두 포함되며 세 씬의 현재 문자열에 누락 글자는 없다. Ingame 전체 TMP 텍스트의 overflow 검사와 Login/HUD/배낭 실행 캡처를 확인했다. 현재 Console 오류/경고 0. 빌드 및 Git 작업 없음.

원본 폰트 내 저작권 및 라이선스 전문은 LicenseMetadata.txt에 함께 보존했다. 클라이언트 내부 메타데이터 증거: Tools/UIReview/DkbiFontEvidence.json.
