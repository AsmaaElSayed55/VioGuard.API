1. Project Overview :
VioGuard API is a backend system designed to ensure digital safety by identifying and filtering out violent content. It provides automated text and video moderation services, leveraging targeted analytical models to process media submissions. The application tracks user history, evaluates severity levels, and generates detailed statistical safety reports.

2. Core Features :
2.1 Multi-Format Content Handling: Seamless ingestion and separate processing tracks for text blocks and video streams.

2.2 Polymorphic Detection Engines: Distinct AI or programmatic models optimized specifically to find violent terminology in text or flag violent visual sequences in videos.

2.3 Aggregated Analytics Reporting: Quantifiable analytics outlining the safety rating of processed content, tracking total volumes, and detailing safety violation percentages.

2.4 User Auditing & History Logging: Full transparency for registered users to track their historical uploads, processing dates, and detection results.

3. Architecture & Class Breakdown
The system is structured around several distinct components working in harmony:

3.1 User Management Layer :
User Class: Manages user registration/authentication (Email, FullName, Password). It serves as the interaction point allowing individuals to trigger actions like Upload(), CheckReport(), and CheckHistory().

3.2 Content & Ingestion Layer :
- Content (Base Class): An abstract/base representation of uploaded media storing a unique reference (URL), categorizing its format (Type), and stamping a DetectionDate.

- Text_Content: Inherits/extends general content. Captures raw strings (TextContext), compiles lists of flagged toxic phrases (ViolentWords), and yields a final evaluation (ViolentResult).

- Video_Content: Inherits/extends general content. Quantifies flagged video sequences into a weighted intensity score (ViolentPersent).

3.3 The Detection Engine (Model) :
- Model (Base Class): Establishes standard contracts for the processing core with an Id, Name, and lifecycle hooks (Detect(), ShowResult()).

- Text_Detect_Model: Overrides base behaviors to handle Natural Language Processing (NLP) or regex dictionary matching for identifying profanity and physical threats.

- Video_Detect_Model: Overrides base behaviors to integrate computer vision or frame-by-frame analysis to pinpoint visual depictions of violence.

3.4 Analytics & Audit Tracking :
- System: The central coordinator or core pipeline controller responsible for routing data across models, logs, and reports.

- History: Tracks chronological transactional sequences (VideosUploaded, TextUploaded) and formats past evaluations via ShowHistory().

- Report: Aggregates macro safety trends. It tracks numbers of total and violating items (NumOfVideo, ViolentText, etc.) to calculate comprehensive safety yields (ViolentPersent()).